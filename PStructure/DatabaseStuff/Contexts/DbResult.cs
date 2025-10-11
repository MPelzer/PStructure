using System;

public class DbResult
{
    public bool RequestAnswer { get; set; }
    public Exception? RequestException { get; set; }
    public bool SilentThrow { get; set; }

    public void SetBackToDefault()
    {
        RequestAnswer = false;
        RequestException = null;
        SilentThrow = false;
    }
}