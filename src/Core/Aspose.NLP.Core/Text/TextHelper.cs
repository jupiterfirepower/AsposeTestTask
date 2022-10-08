using System.Text.RegularExpressions;

namespace Aspose.NLP.Core.Text
{
    public class TextHelper
    {
        // ! " # $ % & ' ( ) * + , - . / : ; ? @ [ \ ] ^ _ ` { | } ~ 
        private string removePunctuations(string text) => Regex.Replace(text, @"[^\w\d\s]", string.Empty, RegexOptions.Compiled);
    }
}
