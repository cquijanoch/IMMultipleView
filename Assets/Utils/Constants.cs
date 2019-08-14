
using UnityEngine;

public class Constants
{
    /** Property for Hand mode **/
    public static int HAND_NONE_USE = 0;
    public static int HAND_PRIMARY_USE = 1;
    public static int HAND_SECONDARY_USE = 2;

    /** Color for subspace  **/
    public static Color SPACE_COLOR_WITH_CONTROLLER = new Color(0.13034f, 0.6665765f, 0.9528301f, 0.1f);
    public static Color SPACE_COLOR_WITHOUT_CONTROLLER = new Color(1f, 1f, 1f, 0f);
    public static Color SPACE_COLOR_PREPARE_TO_DELETE = new Color(1f, 0.7019f, 0.7019f, 0.15f);
    public static int MINIMAL_NUM_POINTS_FOR_STEP_1 = 40;

    /** Type MODE HAND **/
    public static int INT_HAND_MODE_MACRO = 1;
    public static int INT_HAND_MODE_MICRO = 2;
    public static string STR_HAND_MODE_MACRO = "Macro";
    public static string STR_HAND_MODE_MICRO = "Micro";

    /** Constants for Event Hand Macro **/
    public static float MINIMAL_TIME_PER_DOUBLE_TRIGGER = 0.5f;

    /** Tags  **/
    public static string TAG_SUBSBPACE = "Subspace";
    public static string TAG_DATA_SCATTERPLOT = "DataScatterplot";

    /** Color DataObject    **/
    public static Color COLOR_DATA_OBJECT_SELECTED = Color.green;
    public static float TRANSPARENCY_DATA = 0.0f;

    public static Color BUTTON_COLOR_ACTIVATE = Color.green;
    public static Color BUTTON_COLOR_DESACTIVATE = Color.white;

}
