using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.EventSystems;
using System.Text;
using Debug = UnityEngine.Debug;

public static class GameTool
{   
    #region EXCUTE_FUNCTION

    public static void Log(object message = null)
    {
        // LogCall(message);
        UnityEngine.Debug.Log(message);
    }
    // public static void LogCall(
    //     object message
    //     , [System.Runtime.CompilerServices.CallerLineNumber] int line = 0
    //     , [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
    //     , [System.Runtime.CompilerServices.CallerFilePath] string filePath = ""
    // )
    // {
    //     UnityEngine.Debug.Log($"{line} :: {memberName} :: {filePath} :: {message}");
    // }
    
    public static bool IsNull(this object obj)
        => obj == null || ReferenceEquals(obj, null) || obj.Equals(null);
    public static Vector2 ZRotationToVector2(float z)
    {
        float fRotation = z * Mathf.Deg2Rad;
        float fX = Mathf.Sin(fRotation);
        float fY = Mathf.Cos(fRotation);

        return new Vector2(fY, fX); ;
    }
    public static float CalculateVelocity(float distance, float time)
    {
        return distance / time;
    }
    public static float CalculateAcceleration(float deltaVelocity, float deltaTime)
    {
        return deltaVelocity / deltaTime;
    }
    public static Vector2 GetRandomDirection()
    {
        float x = UnityEngine.Random.Range(-1.0f, 1.0f);
        float y;

        if (UnityEngine.Random.value < 0.5f)
        {
            y = 1.0f - Mathf.Abs(x);
        }
        else
        {
            y = -1.0f + Mathf.Abs(x);
        }

        return new Vector2(x, y).normalized;
    }
    public static void ResetOfflineTime(DateTime date)
    {
        PlayerPrefs.SetString("LastOffline", date.ToString());
    }
    public static T[] RemoveFirstArrayElement<T>(this T[] original)
    {
        if (original.Length == 0)
        {
            Debug.LogError("Array is empty");
        }
        return original.Skip(1).ToArray();
    }
    public static T[] RemoveArrayElement<T>(this T[] original, int index)
    {
        if (index < 0 || index >= original.Length)
        {
            Debug.LogError(" Index is out of range. Index: " + index + " Array Length: " + original.Length);
        }
        return original.Where((value, i) => i != index).ToArray();
    }
    public static T[] AddItemToArray<T>(this T[] original, T itemToAdd)
    {
        return original.Concat(new[] { itemToAdd }).ToArray();
    }
    public static bool UnityInternetAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static float GetRandomInRange(float value_1, float value_2)
    {
        return value_1 < value_2 ? UnityEngine.Random.Range(value_1, value_2) : UnityEngine.Random.Range(value_2, value_1);
    }
    public static T[] GetRandomElements<T>(T[] array, int count)
    {
        if (array == null || array.Length == 0 || count <= 0)
        {
            return new T[0];
        }

        if (count >= array.Length)
        {
            return array;
        }

        List<T> elements = array.ToList();
        List<T> result = new List<T>();

        System.Random random = new System.Random();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = random.Next(0, elements.Count);
            result.Add(elements[randomIndex]);
            elements.RemoveAt(randomIndex);
        }

        return result.ToArray();
    }
    public static T GetRandomEnumValue<T>()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        T[] values = (T[])Enum.GetValues(typeof(T));
        int randomIndex = UnityEngine.Random.Range(0, values.Length);

        return values[randomIndex];
    }
    public static T[,] ConvertArrayTo2D<T>(T[] array, int rows, int columns)
    {
        if (array.Length != rows * columns)
        {
            throw new ArgumentException("Invalid array size");
        }

        T[,] result = new T[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[i, j] = array[i * columns + j];
            }
        }

        return result;
    }

    public static T[] ConvertArrayTo1D<T>(T[,] array)
    {
        int rows = array.GetLength(0);
        int columns = array.GetLength(1);

        T[] result = new T[rows * columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[i * columns + j] = array[i, j];
            }
        }

        return result;
    }   
#endregion

    #region CONVERT
    public static string ConvertToTens(this int origin)
    {
        if (origin < 10)
        {
            return "0" + origin.ToString();
        }
        else
        {
            return origin.ToString();
        }
    }
    public static string MinuteToHour(this int minutes)
    {
        string time = "";
        if ((int)(minutes / 60.0f) >= 10)
        {
            time += ((int)(minutes / 60.0f)).ToString();
        }
        else
        {
            time += "0" + ((int)(minutes / 60.0f)).ToString();
        }
        if ((int)(minutes % 60) >= 10)
        {
            time += "h" + ((int)(minutes % 60)).ToString() + "m";
        }
        else
        {
            time += "h0" + ((int)(minutes % 60)).ToString() + "m";
        }
        return time;
    }
    public static string SecondsToMinute(this int seconds)
    {
        //if (seconds >= 60)
        //{
        //    return ((int)(seconds / 60.0f)).ToString() + "m" + ((int)(seconds % 60)).ToString() + "s";
        //}
        //return seconds.ToString() + "s";

        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }
    public static string SecondsToHour(this int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    static string[] prefixes = { "", "K", "M", "B", "T", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT",
                                    "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT",
                                    "BU", "BV", "BW", "BX", "BY", "BZ"};
    public static string ToMoney(this double _money)
    {

        int index = 0;
        double newValue = _money;
        if (newValue < 1000)
        {
            return Math.Round(_money).ToString();
        }

        while (newValue >= 1000)
        {
            newValue /= 1000;
            index++;
        }
        return $"{newValue:0.#}{prefixes[index]}";
    }
    public static string RankIntToText(this int index)
    {
        switch (index + 1)
        {
            case 1:
                return "1st";
            case 2:

                return "2nd";
            case 3:

                return "3rd";
            default:
                return index.ToString() + "th";
        }
    }
    public static float ConvertPixelsToDp(float px)
    {
        float density = Screen.dpi / 160f;
        return px / density;
    }
    public static string StringToUnicode(string input)
    {
        //int utf32 = char.ConvertToUtf32(input, 0);
        //return $"{utf32:X4}".ToLower();
        StringBuilder unicodeString = new StringBuilder();
        int i = 0;
        while (i < input.Length)
        {
            char c = input[i];
            int utf32 = char.ConvertToUtf32(input, i);
            if (utf32 > 0xFFFF)
            {
                unicodeString.AppendFormat("{0:X4}-", utf32);
                i++;
            }
            else
            {
                unicodeString.AppendFormat("{0:X4}-", utf32);
            }
            i++;
        }
        return unicodeString.ToString().TrimEnd('-').ToLower();
    }
    #endregion
    #region Check mouse on UI element
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        foreach (RaycastResult curRaysastResult in eventSystemRaysastResults)
        {
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }

        }

        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    public static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
#endregion
}