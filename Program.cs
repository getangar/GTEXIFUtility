﻿using ImageMagick;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageMagick;

string directoryPath = @"C:\Users\genna\OneDrive\Desktop\Test";
string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".heic" };

foreach (string filePath in Directory.GetFiles(directoryPath)) {
    if (IsImageFile(filePath, imageExtensions)) {
        DateTime originalDateTime = GetOriginalDateTime(filePath);
                    
        if (originalDateTime != DateTime.MinValue) {
            File.SetCreationTime(filePath, originalDateTime);
            Console.WriteLine($"Modified creation date of {filePath} to {originalDateTime}");
        }
        else {
            Console.WriteLine($"Failed to read EXIF data from {filePath}");
        }
    }
}

System.Console.WriteLine("Press <ENTER> to quit...");
System.Console.ReadLine();

// Check if the file is a Photo
static bool IsImageFile(string filePath, string[] imageExtensions) {
    string extension = Path.GetExtension(filePath).ToLower();
            
    foreach (string imageExtension in imageExtensions) {
        if (extension == imageExtension) {
            return true;
        }
    }
            
    return false;
}

// Get the EXIF Tag "OriginalDateTime"
static DateTime GetOriginalDateTimeOld(string filePath) {
    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    using (Image image = Image.FromStream(fs, false, false))
    {
        foreach (PropertyItem propItem in image.PropertyItems)
        {
            if (propItem.Id == 0x9003)
            { // DateTimeOriginal tag
                string dateTaken = System.Text.Encoding.ASCII.GetString(propItem.Value).Trim('\0');

                return DateTime.ParseExact(dateTaken, "yyyy:MM:dd HH:mm:ss", null);
            }
        }
    }
            
    return DateTime.MinValue;
}
static DateTime GetOriginalDateTime(string filePath)
{
    using (MagickImage image = new MagickImage(filePath))
    {
        if (image.GetExifProfile() != null)
        {
            DateTime originalDateTime;
            if (DateTime.TryParseExact(image.GetAttribute("exif:DateTimeOriginal"), "yyyy:MM:dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out originalDateTime))
            {
                return originalDateTime;
            }
        }
    }
    return DateTime.MinValue;
}
