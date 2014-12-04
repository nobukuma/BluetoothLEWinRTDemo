// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using Microsoft.Practices.Prism.StoreApps;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace StrawhatNet.BLEDemo.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : VisualStateAwarePage
    {
        new object DataContext { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
