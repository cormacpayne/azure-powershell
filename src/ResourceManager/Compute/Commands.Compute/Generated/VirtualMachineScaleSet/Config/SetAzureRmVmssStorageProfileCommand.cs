// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// Warning: This code was generated by a tool.
// 
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using Microsoft.Azure.Commands.Compute.Automation.Models;
using Microsoft.Azure.Management.Compute.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Compute.Automation
{
    [Cmdlet("Set", "AzureRmVmssStorageProfile", SupportsShouldProcess = true)]
    [OutputType(typeof(PSVirtualMachineScaleSet))]
    public partial class SetAzureRmVmssStorageProfileCommand : Microsoft.Azure.Commands.ResourceManager.Common.AzureRMCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PSVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public string ImageReferencePublisher { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        public string ImageReferenceOffer { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 3,
            ValueFromPipelineByPropertyName = true)]
        public string ImageReferenceSku { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 4,
            ValueFromPipelineByPropertyName = true)]
        public string ImageReferenceVersion { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 5,
            ValueFromPipelineByPropertyName = true)]
        [Alias("Name")]
        public string OsDiskName { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 6,
            ValueFromPipelineByPropertyName = true)]
        public CachingTypes? OsDiskCaching { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 7,
            ValueFromPipelineByPropertyName = true)]
        public DiskCreateOptionTypes? OsDiskCreateOption { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 8,
            ValueFromPipelineByPropertyName = true)]
        public OperatingSystemTypes? OsDiskOsType { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 9,
            ValueFromPipelineByPropertyName = true)]
        public string Image { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 10,
            ValueFromPipelineByPropertyName = true)]
        public string[] VhdContainer { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public string ImageReferenceId { get; set; }

        [Parameter(
            Mandatory = false)]
        public SwitchParameter OsDiskWriteAccelerator { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public Microsoft.Azure.Management.Compute.Models.StorageAccountTypes? ManagedDisk { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true)]
        public VirtualMachineScaleSetDataDisk[] DataDisk { get; set; }

        protected override void ProcessRecord()
        {
            if (ShouldProcess("VirtualMachineScaleSet", "Set"))
            {
                Run();
            }
        }

        private void Run()
        {
            if (this.ImageReferencePublisher != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // ImageReference
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference = new Microsoft.Azure.Management.Compute.Models.ImageReference();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference.Publisher = this.ImageReferencePublisher;
            }

            if (this.ImageReferenceOffer != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // ImageReference
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference = new Microsoft.Azure.Management.Compute.Models.ImageReference();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference.Offer = this.ImageReferenceOffer;
            }

            if (this.ImageReferenceSku != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // ImageReference
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference = new Microsoft.Azure.Management.Compute.Models.ImageReference();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference.Sku = this.ImageReferenceSku;
            }

            if (this.ImageReferenceVersion != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // ImageReference
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference = new Microsoft.Azure.Management.Compute.Models.ImageReference();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference.Version = this.ImageReferenceVersion;
            }

            if (this.ImageReferenceId != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // ImageReference
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference = new Microsoft.Azure.Management.Compute.Models.ImageReference();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.ImageReference.Id = this.ImageReferenceId;
            }

            if (this.OsDiskName != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.Name = this.OsDiskName;
            }

            if (this.OsDiskCaching != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.Caching = this.OsDiskCaching;
            }

            // VirtualMachineProfile
            if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
            {
                this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
            }
            // StorageProfile
            if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
            {
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
            }
            // OsDisk
            if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
            {
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
            }
            this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.WriteAcceleratorEnabled = this.OsDiskWriteAccelerator.IsPresent;

            if (this.OsDiskCreateOption.HasValue)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }

                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.CreateOption = this.OsDiskCreateOption.Value;
            }

            if (this.OsDiskOsType != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.OsType = this.OsDiskOsType;
            }

            if (this.Image != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                // Image
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.Image == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.Image = new Microsoft.Azure.Management.Compute.Models.VirtualHardDisk();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.Image.Uri = this.Image;
            }

            if (this.VhdContainer != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.VhdContainers = this.VhdContainer;
            }

            if (this.ManagedDisk != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                // OsDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetOSDisk();
                }
                // ManagedDisk
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.ManagedDisk == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.ManagedDisk = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetManagedDiskParameters();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.OsDisk.ManagedDisk.StorageAccountType = this.ManagedDisk;
            }

            if (this.DataDisk != null)
            {

                // VirtualMachineProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetVMProfile();
                }
                // StorageProfile
                if (this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile == null)
                {
                    this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile = new Microsoft.Azure.Management.Compute.Models.VirtualMachineScaleSetStorageProfile();
                }
                this.VirtualMachineScaleSet.VirtualMachineProfile.StorageProfile.DataDisks = this.DataDisk;
            }

            WriteObject(this.VirtualMachineScaleSet);
        }
    }
}

