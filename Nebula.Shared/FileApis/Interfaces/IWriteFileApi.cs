﻿namespace Nebula.Shared.FileApis.Interfaces;

public interface IWriteFileApi
{
    public bool Save(string path, Stream input);
    public bool Remove(string path);
    public bool Has(string path);
}