using UnityEngine;
using System.Text;

public static class StringExtension 
{
    public static string GetShortNumberForm(this string s)
    {
        StringBuilder formattedText = new StringBuilder(s);
        
        int currentNumberCount = 0;
        for (int i = formattedText.Length - 1; i >= 0; i--)
        {
            bool digitCharacter = false;
            var curChar = formattedText[i];
            if (curChar >= '0' && curChar <= '9')
            {
                digitCharacter = true;
                currentNumberCount += 1;
            }
            if ((!digitCharacter) || ((digitCharacter) && (i == 0)))
            {
                if (currentNumberCount > 3)
                {
                    int numberStartIdx = (digitCharacter) ? i : (i + 1);

                    int lettersToCut = currentNumberCount > 6 ? 6 : 3;

                    string endLetter = lettersToCut == 6 ? "M" : "k";

                    int numberFinishIdx = numberStartIdx + currentNumberCount;
                    int pointPlace = numberFinishIdx - lettersToCut;
                    int cutStartIdx = pointPlace;
                    if (formattedText[cutStartIdx] != '0' && ((currentNumberCount - lettersToCut) <= 2))
                    {
                        formattedText.Insert(pointPlace, ".");
                        cutStartIdx += 2;
                        lettersToCut -= 1;
                    }
                    formattedText.Remove(cutStartIdx, lettersToCut);
                    formattedText.Insert(cutStartIdx, endLetter);
                }
                currentNumberCount = 0;
            }
        }

        return formattedText.ToString();
    }
	
}
