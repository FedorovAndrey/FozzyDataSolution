﻿namespace SLPDBLibrary.Models;

public partial class Enumeration
{
    public string Key { get; set; } = null!;

    public int Enumvalue { get; set; }

    public string? Enumtext { get; set; }

    public string Languageid { get; set; } = null!;
}
