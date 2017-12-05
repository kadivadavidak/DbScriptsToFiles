using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DbScriptsToGit
{
    class SaveScripts
    {
        internal static void Execute()
        {
            // Get a dictionary of DB module names and scripts : Dictionary<string name, string script.
            var scripts = ScriptsToFile();

            // Save each of the scripts to a file.
            foreach (var script in scripts)
            {
                SaveToFile(script.Key, script.Value);
            }
        }

        internal static Dictionary<string, string> ScriptsToFile()
        {
            try
            {
                // Get a list of databases to save scripts for
                var databases = DataHandler.DatabaseNamesToList();
                var moduleDict = new Dictionary<string, string>();

                // Iterate through each of the databases and get all of the scripts and their names and save them to a dictionary.
                foreach (var database in databases)
                {
                    var modules = DataHandler.ModulesToDataSet(database);

                    foreach (DataRow row in modules.Rows)
                    {
                        moduleDict.Add(row[1].ToString(), row[0].ToString());
                    }
                }

                // Return dictuinary of the script names and definitions.
                return moduleDict;
            }
            catch (Exception e)
            {
                // Send email if there is an error.
                SmptHandler.SendMessage("[ScriptsToGit Error] Unable to get list of datbases.", $"There was a problem geting list of columns from database for the following reason:\n\n{e}");
                throw;
            }
        }

        internal static void SaveToFile(string fileName, string contents)
        {
            var filePath = $"\\{fileName}.sql";

            // Save scripts to file. If there is an error, send email reporting it.
            try
            {
                File.WriteAllText(filePath, contents);
            }
            catch (Exception e)
            {
                SmptHandler.SendMessage($"[ScriptsToGit Error] Unable to save file {fileName} to file share.", $"The file {fileName} could not be saved to the file share for backup to Git. The following error occured: \n\n{e}");
                throw;
            }
        }
    }
}
