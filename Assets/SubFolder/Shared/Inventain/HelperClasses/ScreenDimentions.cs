using UnityEngine;
using System.Collections;

public static class ScreenDimentions
{
    public const int iPhone6Height = 1920;

    public static int Height
    {
        get
        {
            int h = Screen.height;           

            #if UNITY_EDITOR

            float width;
            float height;
            float aspect;

            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            h = (int)height;       

            #endif

            if (Screen.height == iPhone6Height)
            {
                h = 2318;
            }

            if (Screen.width == iPhone6Height)
            {
                h = 1304;
            }

            return h;
        }
    }

    public static int Width
    {
        get
        {
            int w = Screen.width;

            #if UNITY_EDITOR

            float width;
            float height;
            float aspect;

            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            w = (int)width;
           
            #endif

            if (Screen.height == iPhone6Height)
            {
                w = 1304;
            }

            if (Screen.width == iPhone6Height)
            {
                w = 2318;
            }

            return w;
        }
    }
}
