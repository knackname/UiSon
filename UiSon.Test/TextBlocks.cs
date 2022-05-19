using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonTextBlock("TextBlock 1")]
    [UiSonTextBlock("TextBlock 2")]
    [UiSonTextBlock("TextBlock 3")]
    public class TextBlocks
    {
        [UiSonLabelUi]
        string labelTest = "LabelTestWOOOOO";
    }
}
