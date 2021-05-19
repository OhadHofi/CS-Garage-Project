﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    public class Garage
    {
        private Dictionary<string, VehicleInformation> m_GarageVehicles;

        public Garage()
        {
            m_GarageVehicles = new Dictionary<string, VehicleInformation>();
        }

        private const int k_MaxCarFuelTAnk = 45;
        private const int k_MaxMotorcycleFuelTAnk = 6;
        private const int k_MaxTruckFuelTAnk = 120;
        private const float k_MaxBatteryCar = 3.2f;
        private const float k_MaxBatteryMotorcycle = 1.8f;

        public enum eVehiclesTyps { car=1, motorcycle, truck }
        public enum eEngineTyps { gasEngine = 1, electricEngine }


        public static string getVehickesTyps()
        {
            StringBuilder resString = new StringBuilder();
            int i = 1;
            foreach (string s in Enum.GetNames(typeof(eVehiclesTyps)))
            {
                resString.AppendFormat(@"{0} - {1}
", i, s);
                i++;
            }
            return resString.ToString();
        }

        public static string getOptionOfEngine(string i_vehicleType)
        {
            StringBuilder resString = new StringBuilder();
            if(i_vehicleType.Equals(eVehiclesTyps.car)|| i_vehicleType.Equals(eVehiclesTyps.motorcycle))
            {
                resString.AppendFormat(@"1- gas engine
2- electric engine");
            }
            else if(i_vehicleType.Equals(eVehiclesTyps.truck))
            {
                resString.AppendFormat(@"1- gas engine");
            }

            return resString.ToString();
        }

        public static Engine addEngine(string i_EngineType, string i_vehicleType)
        {
            Engine resEngine;
            if (i_EngineType.Equals(eEngineTyps.electricEngine))
            {
                if(i_vehicleType.Equals(eVehiclesTyps.car))
                {
                    GasEngine engineForCar = new GasEngine(GasEngine.eFuelTypes.Octan95, k_MaxCarFuelTAnk);
                    resEngine = engineForCar;
                }
                else if (i_vehicleType.Equals(eVehiclesTyps.motorcycle))
                {
                    GasEngine engineForMotorcycle = new GasEngine(GasEngine.eFuelTypes.Octan98, k_MaxMotorcycleFuelTAnk);
                    resEngine = engineForMotorcycle;
                }
                else//truck
                {
                    GasEngine engineForTruck = new GasEngine(GasEngine.eFuelTypes.Soler, k_MaxTruckFuelTAnk);
                    resEngine = engineForTruck;
                }
            }
            else
            {
                if (i_vehicleType.Equals(eVehiclesTyps.car))
                {
                    ElectricEngine engineForCar = new ElectricEngine(k_MaxBatteryCar);
                    resEngine = engineForCar;
                }
                else//motorcycle
                {
                    ElectricEngine engineForMotorcycle = new ElectricEngine(k_MaxBatteryMotorcycle);
                    resEngine = engineForMotorcycle;
                }
            }
            return resEngine;
        }
        public bool AddVehicle(Vehicle i_Vehicle, string i_OwnerName, string i_OwnerPhone)
        {
            bool added = false;

            if(!m_GarageVehicles.ContainsKey(i_Vehicle.LicenseNumber))
            {
                added = true;
                m_GarageVehicles.Add(
                                i_Vehicle.LicenseNumber,
                                new VehicleInformation(i_Vehicle, i_OwnerName, i_OwnerPhone));
            }
            else
            {
                m_GarageVehicles[i_Vehicle.LicenseNumber].CurrentState = VehicleInformation.eVehicleState.Repairing;
            }

            return added;
        }

        public static Vehicle addVehicle(string io_modelName, string io_licenseNumber, Wheel i_wheel, Engine i_engine, string i_vehicleType)
        {
            Vehicle resVehicle;
            if (i_vehicleType.Equals(eVehiclesTyps.car))
            {
                Car car = new Car(i_engine, io_modelName, io_licenseNumber, i_wheel);
                resVehicle = car;
            }
            else if(i_vehicleType.Equals(eVehiclesTyps.motorcycle))
            {
                Motorcycle motorcycle = new Motorcycle(i_engine, io_modelName, io_licenseNumber, i_wheel);
                resVehicle = motorcycle;
            }
            else//truck
            {
                Truck truck = new Truck(i_engine, io_modelName, io_licenseNumber, i_wheel);
                resVehicle = truck;
            }

            return resVehicle;
        }


        public string GetLicenseNumbers(bool i_FilterByState, VehicleInformation.eVehicleState i_State)
        {
            StringBuilder licenses = new StringBuilder();
            int licenseCount = 1;

            foreach(KeyValuePair<string, VehicleInformation> vehicle in m_GarageVehicles)
            {
                if(!i_FilterByState)
                 {
                    licenses.AppendFormat(@"{0}. {1}{2}",
                            licenseCount++,
                            vehicle.Value.GetVehicle.LicenseNumber,
                            Environment.NewLine);
                 }
                else if (vehicle.Value.CurrentState == i_State)
                {
                    licenses.AppendFormat(@"{0}. {1}{2}",
                            licenseCount++,
                            vehicle.Value.GetVehicle.LicenseNumber,
                            Environment.NewLine);
                }
            }

            return licenses.ToString();
        }

        public void ChangeVehicleState(VehicleInformation.eVehicleState i_NewState, string i_LicenseNumber)
        {
            if(!m_GarageVehicles.ContainsKey(i_LicenseNumber))
            {
                throw new ArgumentException("No matching vehicle found in the garage");
            }

            m_GarageVehicles[i_LicenseNumber].CurrentState = i_NewState;
        }

        public void InflateWheels(string i_LicenseNumber)
        {
            if (!m_GarageVehicles.ContainsKey(i_LicenseNumber))
            {
                throw new ArgumentException("No matching vehicle found in the garage");
            }

            List<Wheel> wheels = m_GarageVehicles[i_LicenseNumber].GetVehicle.Wheels;
            foreach(Wheel wheel in wheels)
            {
                wheel.Inflate();
            }
        }

        public void FuelVehicle(float i_ToAdd, GasEngine.eFuelTypes i_FuelType, string i_LicenseNumber)
        {
            if(!m_GarageVehicles.ContainsKey(i_LicenseNumber))
            {
                throw new ArgumentException("No matching vehicle found in the garage");
            }

            Vehicle toFuel = m_GarageVehicles[i_LicenseNumber].GetVehicle;
            if(toFuel.Engine is GasEngine)
            {
                (toFuel.Engine as GasEngine).Fuel(i_FuelType, i_ToAdd, toFuel);
            }
            else
            {
                throw new ArgumentException("Vehicle requested is not fuel operated");
            }
        }

        public void ChargeVehicle(float i_ToAdd, string i_LicenseNumber)
        {
            if (!m_GarageVehicles.ContainsKey(i_LicenseNumber))
            {
                throw new ArgumentException("No matching vehicle found in the garage");
            }

            Vehicle toFuel = m_GarageVehicles[i_LicenseNumber].GetVehicle;
            if (toFuel.Engine is ElectricEngine)
            {
                (toFuel.Engine as ElectricEngine).Charge(i_ToAdd, toFuel);
            }
            else
            {
                throw new ArgumentException("Vehicle requested is not electrically operated");
            }
        }

        public string GetFullVehicleInfo(string i_LicenseNumber)
        {
            if (!m_GarageVehicles.ContainsKey(i_LicenseNumber))
            {
                throw new ArgumentException("No matching vehicle found in the garage");
            }

            VehicleInformation toShow = m_GarageVehicles[i_LicenseNumber];
            StringBuilder vehicleInfo = new StringBuilder();
            vehicleInfo.AppendFormat(@"===========================
Information for {0}'s vehicle
===========================
Phone number - {1}
State - {2}",
                        toShow.OwnerName,
                        toShow.OwnerPhone,
                        toShow.CurrentState);

            vehicleInfo.Append(toShow.GetVehicle.ToString());
            return vehicleInfo.ToString();
        }




        public class VehicleInformation
        {
            public enum eVehicleState
            {
                Repairing,
                Repaired,
                Paid
            }

            private Vehicle m_Vehicle;
            private string m_OwnerName;
            private string m_OwnerPhone;
            private eVehicleState m_CurrentState;

            public VehicleInformation(Vehicle i_Vehicle, string i_OwnerName, string i_OwnerPhone)
            {
                m_Vehicle = i_Vehicle;
                m_OwnerName = i_OwnerName;
                m_OwnerPhone = i_OwnerPhone;
                m_CurrentState = eVehicleState.Repairing;
            }

            public eVehicleState CurrentState
            {
                get { return m_CurrentState; }
                set { m_CurrentState = value; }
            }

            public Vehicle GetVehicle
            {
                get { return m_Vehicle; }
            }

            public string OwnerName
            {
                get { return m_OwnerName; }
                set { m_OwnerName = value; }
            }

            public string OwnerPhone
            {
                get { return m_OwnerPhone; }
                set { m_OwnerPhone = value; }
            }
        }
    }
}
