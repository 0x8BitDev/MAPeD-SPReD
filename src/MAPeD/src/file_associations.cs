/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2020
 * Time: 15:20
 */
using System;
using System.Diagnostics;
using Microsoft.Win32;

// The file association classes implemented by Kirill Osenkov (https://stackoverflow.com/a/44816953)

namespace MAPeD
{
	/// <summary>
	/// Description of drawable_base.
	/// </summary>
	public class FileAssociation
	{
	    public string Extension { get; set; }
	    public string ProgId { get; set; }
	    public string FileTypeDescription { get; set; }
	    public string ExecutableFilePath { get; set; }
	}
	
	public class FileAssociations
	{
	    // needed so that Explorer windows get refreshed after the registry is updated
	    [System.Runtime.InteropServices.DllImport("Shell32.dll")]
	    private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
	
	    private const int SHCNE_ASSOCCHANGED = 0x8000000;
	    private const int SHCNF_FLUSH = 0x1000;
	
	    public static void EnsureAssociationsSet()
	    {
	        var filePath = Process.GetCurrentProcess().MainModule.FileName;
	        EnsureAssociationsSet(
	            new FileAssociation
	            {
#if DEF_NES		
	                Extension = ".mapednes",
#elif DEF_SMS
	                Extension = ".mapedsms",
#endif		
	                ProgId = utils.CONST_PLATFORM + "_GameMapsEditor",
	                FileTypeDescription = utils.CONST_PLATFORM + " Game Maps Editor File",
	                ExecutableFilePath = filePath
	            });
	    }
	
	    public static void EnsureAssociationsSet(params FileAssociation[] associations)
	    {
	        bool madeChanges = false;
	        foreach (var association in associations)
	        {
	            madeChanges |= SetAssociation(
	                association.Extension,
	                association.ProgId,
	                association.FileTypeDescription,
	                association.ExecutableFilePath);
	        }
	
	        if (madeChanges)
	        {
	            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
	        }
	    }
	
	    public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
	    {
	        bool madeChanges = false;
	        madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
	        madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
	        madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId + @"\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
	        return madeChanges;
	    }
	
	    private static bool SetKeyDefaultValue(string keyPath, string value)
	    {
	        using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
	        {
	            if (key.GetValue(null) as string != value)
	            {
	                key.SetValue(null, value);
	                return true;
	            }
	        }
	
	        return false;
	    }	 
	}	
}