
using UnityEngine;

public class Constants
{
    /** Property for Hand mode **/
    public static int HAND_NONE_USE = 0;
    public static int HAND_PRIMARY_USE = 1;
    public static int HAND_SECONDARY_USE = 2;

    /** Color for subspace  **/
    public static Color SPACE_COLOR_WITH_CONTROLLER = new Color(0.6666527f, 0.6581524f, 0.1f, 0.2941177f);
    public static Color SPACE_COLOR_WITHOUT_CONTROLLER = new Color(0.6666527f, 0.6581524f, 0.9622641f, 0.2941177f);
    public static Color SPACE_COLOR_PREPARE_TO_DELETE = new Color(1f, 0.7019f, 0.7019f, 0.2941177f);
    public static int MINIMAL_NUM_POINTS_FOR_STEP_1 = 40;

    /** Type MODE HAND **/
    public static int INT_HAND_MODE_MACRO = 1;
    public static int INT_HAND_MODE_MICRO = 2;
    public static string STR_HAND_MODE_MACRO = "Macro";
    public static string STR_HAND_MODE_MICRO = "Micro";

    /** Constants for Event Hand Macro **/
    public static float MINIMAL_TIME_PER_DOUBLE_TRIGGER = 0.5f;
}
