// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


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
    // This QuickStart is documented at http://go.microsoft.com/fwlink/?LinkID=288830&clcid=0x409

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
            NavigateCommand = new DelegateCommand(() => Initialize());

            dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
        }

        public DelegateCommand NavigateCommand { get; set; }

        public List<string> DisplayItems
        {
            get { return dataRepository.GetFeatures(); }
        }

        // http://msdn.microsoft.com/ja-jp/library/windows/apps/dn263090.aspx
        // http://msdn.microsoft.com/en-us/library/windows/apps/xaml/Dn264584(v=win.10).aspx
        double convertTemperatureData(byte[] temperatureData)
        {
            //    // Read temperature data in IEEE 11703 floating point format
            //    // temperatureData[0] contains flags about optional data - not used
            //    UInt32 mantissa = ((UInt32)temperatureData[3] << 16) |
            //        ((UInt32)temperatureData[2] << 8) |
            //        ((UInt32)temperatureData[1]);

            //    Int32 exponent = (Int32)temperatureData[4];

            //    return mantissa * Math.Pow(10.0, exponent);

            UInt32 temp = (UInt32)(temperatureData[4] << 24) | (UInt32)(temperatureData[3] << 8)
                | (UInt32)(temperatureData[2] << 8) | (UInt32)temperatureData[1];
            UInt32 mantissa = temp & 0x00FFFFFF;
            sbyte exponent = (sbyte)((temp >> 24) & 0x000000FF);

            double temparature = (double)mantissa * Math.Pow(10, exponent);
            return temparature;
        }

        /*
            uint32_t temp_ieee11073 = quick_ieee11073_from_float(temperature);
            memcpy(thermTempPayload+1, &temp_ieee11073, 4);
            ble.updateCharacteristicValue(tempChar.getValueAttribute().getHandle(), thermTempPayload, sizeof(thermTempPayload));
         *  ...
         
            uint8_t  exponent = 0xFF; //exponent is -1
            uint32_t mantissa = (uint32_t)(temperature*10);
    
            return ( ((uint32_t)exponent) << 24) | mantissa;
         */


        async void Initialize()
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

                    var tmp = firstThermometerService.GetCharacteristics(
                        GattCharacteristicUuids.TemperatureMeasurement);

                    GattCharacteristic thermometerCharacteristic
                        = firstThermometerService.GetCharacteristics(
                            GattCharacteristicUuids.TemperatureMeasurement)[0];

                    thermometerCharacteristic.ValueChanged += temperatureMeasurementChanged;

                    await thermometerCharacteristic
                        .WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                    
                }
                else
                {
                    // 温度計サービスを見つけられなかった
                    // Capabilityの設定漏れはここへ
                    return;
                }
            }
            else
            {
                // 発見できなかった
                // BluetoothがOFFの場合はここへ
                return;
            }
        }

        async void temperatureMeasurementChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs eventArgs)
        {
            byte[] temperatureData = new byte[eventArgs.CharacteristicValue.Length];
            Windows.Storage.Streams.DataReader.FromBuffer(
                eventArgs.CharacteristicValue).ReadBytes(temperatureData);

            var temperatureValue = convertTemperatureData(temperatureData);

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    TemperatureText = temperatureValue.ToString();
                });
        }


    }
}
