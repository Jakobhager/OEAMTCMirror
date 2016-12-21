using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace OEAMTCMirror
{
    class RegistryWrapper
    {
        private const string _userRoot = "HKEY_CURRENT_USER";
        private const string _computerRoot = "HKEY_LOCAL_MACHINE";
        private const string _keyFolder = "OEAMTCMirror";

        private const string _fullPath = _userRoot + "\\" + _keyFolder;


        public RegistryWrapper()
        {
            string checkExists = (string)Registry.GetValue(_fullPath, "Initiated", "notexisting");

            if (checkExists == null)
            {
                WriteString("Initiated", "true");
            }
        }

        public string ReadString(string key)
        {
            try
            {
                string valueFromReg = (string)Registry.GetValue(_fullPath, key, "notexisting");
                return valueFromReg;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string[] ReadArray(string key)
        {
            try
            {
                string[] tArray = (string[])Registry.GetValue(_fullPath, key, new string[] { "notexisting" });
                return tArray;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void WriteString(string key, string value)
        {
            try
            {
                Registry.SetValue(_fullPath, key, value);
            }
            catch (Exception ex)
            {

            }
        }

        public void WriteArray(string key, string[] values)
        {
            try
            {
                Registry.SetValue(_fullPath, key, values);
            }
            catch (Exception ex)
            {

            }
        }

        public bool Exists(string key)
        {
            try
            {
                var checkExists = Registry.GetValue(_fullPath, key, "notexisting");

                if (checkExists != "notexisting")
                {
                    return true;
                }
                else
                {
                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
