using StrawhatNet.BLEDemo.Services;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Core;

namespace StrawhatNet.BLEDemo.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        IDataRepository dataRepository;

        public string serviceNameText;
        public string ServiceNameText
        {
            get
            {
                return this.serviceNameText;
            }
            set{
                SetProperty(ref serviceNameText, value);
            }
        }

        public string temperatureText;
        public string TemperatureText
        {
            get
            {
                return this.temperatureText;
            }
            set
            {
                SetProperty(ref temperatureText, value);
            }
        }

        private CoreDispatcher dispatcher;
        
        public MainPageViewModel(IDataRepository argDataRepository, INavigationService navService)
        {
            dataRepository = argDataRepository;
            NavigateCommand = new DelegateCommand(() => StartMeasurement());

            dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public DelegateCommand NavigateCommand { get; set; }

        private async void StartMeasurement()
        {
            var deveiceSelector = GattDeviceService.GetDeviceSelectorFromUuid(
                GattServiceUuids.HealthThermometer);

            var themometerServices = await Windows.Devices.Enumeration
                .DeviceInformation.FindAllAsync(deveiceSelector, null);

            if (themometerServices.Count > 0)
            {
                var themometerService = themometerServices[0];
                ServiceNameText = "Using service: " + themometerService.Name;

                GattDeviceService firstThermometerService
                    = await GattDeviceService.FromIdAsync(themometerService.Id);

                if (firstThermometerService != null)
                {
                    GattCharacteristic thermometerCharacteristic
                        = firstThermometerService.GetCharacteristics(
                            GattCharacteristicUuids.TemperatureMeasurement)[0];

                    thermometerCharacteristic.ValueChanged += TemperatureMeasurementChanged;

                    await thermometerCharacteristic
                        .WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                }
                else
                {
                    // ���x�v�T�[�r�X���������Ȃ�����
                    // Capability�̐ݒ�R��͂�����
                    return;
                }
            }
            else
            {
                // �����ł��Ȃ�����
                // Bluetooth��OFF�̏ꍇ�͂�����
                return;
            }
        }

        private async void TemperatureMeasurementChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs eventArgs)
        {
            byte[] temperatureData = new byte[eventArgs.CharacteristicValue.Length];
            Windows.Storage.Streams.DataReader.FromBuffer(
                eventArgs.CharacteristicValue).ReadBytes(temperatureData);

            var temperatureValue = ConvertTemperatureData(temperatureData);

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    TemperatureText = temperatureValue.ToString();
                });
        }

        private double ConvertTemperatureData(byte[] temperatureData)
        {
            UInt32 temp = (UInt32)(temperatureData[4] << 24)
                | (UInt32)(temperatureData[3] << 8)
                | (UInt32)(temperatureData[2] << 8)
                | (UInt32)temperatureData[1];
            UInt32 mantissa = temp & 0x00FFFFFF;
            sbyte exponent = (sbyte)((temp >> 24) & 0x000000FF);

            double temparature = (double)mantissa * Math.Pow(10, exponent);
            return temparature;
        }
    }
}
