using System;
using System.Reflection;
using System.Text.Json;

public interface IPBase
{
    string ToStringForTest();

    bool Validate();

    string ToJson();

    T FromJson<T>(string json);

    IPBase Clone();
}