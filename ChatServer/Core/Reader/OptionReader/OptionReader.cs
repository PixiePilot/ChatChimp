using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace MinecraftSharp.Core.Reader.OptionsReader
{
    public class OptionReader
    {
        private string fileExtension = ".opt";

        private int readAttempt = 0;

        private struct EnvStruct
        {
            public string variableType { get; set; }
            public string VariableName { get; set; }
            public string VariableValue { get; set; }
        }

        private List<EnvStruct> envVariables = new List<EnvStruct>();
        public OptionReader(string FileName)
        {
        retry:
            readAttempt++;
            if( readAttempt == 100)
            {
                MessageBox.Show("Check if the program has permissions to write in this folder.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            string options = string.Empty;
            try
            {
                options = File.ReadAllText(FileName + fileExtension);
            }
            catch (Exception) { createDefaultOpt(FileName); goto retry; }
            List<string> tempOptions = new List<string>(options.Split('\n'));
            foreach (string option in tempOptions)
            {
                List<string> tempOption = new List<string>(option.Split(':'));
                EnvStruct env = new EnvStruct();
                env.variableType = tempOption[0];
                env.VariableName = tempOption[1];
                env.VariableValue = tempOption[2];
                envVariables.Add(env);
            }
        }

        public int amountLoaded()
        {
            return envVariables.Count;
        }

        public dynamic getEnv(string variableName)
        {
            if (envVariables.Count == 0)
                return null;

            EnvStruct option = envVariables.Where(env => env.VariableName == variableName).Single();

            switch (option.variableType)
            {
                case "string":
                    return option.VariableValue.Replace("\r", string.Empty);

                case "int":
                    return int.Parse(option.VariableValue);

                case "float":
                    return float.Parse(option.VariableValue);

                case "double":
                    return double.Parse(option.VariableValue);

                case "bool":
                    return bool.Parse(option.VariableValue);

                case "ulong":
                    return ulong.Parse(option.VariableValue);

                default:
                    return null;
            }
        }

        public void createDefaultOpt(string FileName)
        {
            File.WriteAllText(FileName + fileExtension, "string:ip_address:0.0.0.0\r\nint:port:25565\r\nint:max_players:169\r\nint:max_ping:300");
        }
    }
}
