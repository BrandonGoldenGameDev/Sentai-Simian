using UnityEngine;

public abstract class Setting<T>
{
    protected string _key;
    protected T _default;
    protected T _value;
    protected bool loaded = false;

    public T Value { get => Load(); set => Set(value); }
    public string Key { get => _key; }

    public Setting(string key, T def)
    {
        _key = key;
        _default = def;
    }


    protected abstract void Set(T value);
    protected abstract T Load();
}

public class FloatSetting : Setting<float>
{
    public FloatSetting(string key, float def) : base(key, def)
    {
    }

    protected override void Set(float newValue)
    {
        if (newValue == _value)
            return;

        PlayerPrefs.SetFloat(_key, newValue);
        _value = newValue;
    }

    protected override float Load()
    {
        if (loaded)
            return _value;

        _value = PlayerPrefs.GetFloat(_key, _default);
        loaded = true;
        return _value;
    }
}

public class StringSetting : Setting<string>
{
    public StringSetting(string key, string def) : base(key, def)
    {
    }

    protected override string Load()
    {
        if (loaded)
            return _value;

        _value = PlayerPrefs.GetString(_key, _default);
        loaded = true;
        return _value;
    }

    protected override void Set(string value)
    {
        Debug.Log($"Setting {_key} to {value}");

        if (value == _value)
            return;

        PlayerPrefs.SetString(_key, value);
        _value = value;
    }
}

public class IntSetting : Setting<int>
{
    public IntSetting(string key, int def) : base(key, def)
    {
    }

    protected override int Load()
    {
        if (loaded)
            return _value;

        _value = PlayerPrefs.GetInt(_key, _default);
        loaded = true;
        return _value;
    }

    protected override void Set(int value)
    {
        Debug.Log($"Setting {_key} to {value}");

        if (value == _value)
            return;

        PlayerPrefs.SetInt(_key, value);
        _value = value;
    }
}

public class BoolSetting : Setting<bool>
{
    public BoolSetting(string key, bool def) : base(key, def)
    {
    }

    protected override bool Load()
    {
        if (loaded)
            return _value;

        _value = IntToBool(PlayerPrefs.GetInt(_key, BoolToInt(_default)));
        loaded = true;
        return _value;
    }

    protected override void Set(bool value)
    {
        Debug.Log($"Setting {_key} to {value}");

        if (value == _value)
            return;

        PlayerPrefs.SetInt(_key, BoolToInt(value));
        _value = value;
    }

    private int BoolToInt(bool b) => b ? 1 : 0;
    private bool IntToBool(int i) => i == 1;
}

public static class Settings
{
    public static FloatSetting MasterVolume = new FloatSetting("MasterVolume", 0.5f);
    public static FloatSetting SFXVolume = new FloatSetting("SFXVolume", 1f);
    public static FloatSetting MusicVolume = new FloatSetting("MusicVolume", 1f);
    public static FloatSetting CameraShakeAmount = new FloatSetting("CameraShakeAmount", 1f);
    public static StringSetting Rebinds = new StringSetting("Rebinds", string.Empty);
    public static IntSetting Resolution = new IntSetting("Resolution", 0);
    public static IntSetting Vsync = new IntSetting("Vsync", QualitySettings.vSyncCount);
    public static IntSetting TargetFramerate = new IntSetting("TargetFramerate", Application.targetFrameRate);
    public static IntSetting FullscreenMode = new IntSetting("FullscreenMode", (int)Screen.fullScreenMode);
    public static BoolSetting EnableHUD = new BoolSetting("EnableHUD", true);
    public static IntSetting TargetDisplay = new IntSetting("UnitySelectMonitor", 0);

    public static void Apply() => PlayerPrefs.Save();
}
