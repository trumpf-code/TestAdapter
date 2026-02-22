
using System;
using System.Collections.Generic;
using System.Linq;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TcTagsRetriever
{
    public static List<string> GetTags(Type typeOfTestCase)
    {
        List<string> tagsArray = new();

        var attributesExcludingRequirementAttribute = typeOfTestCase
         .GetCustomAttributes(true)
         .Where(a => a is TiRequirement == false).ToList();

        foreach (Attribute attribute in attributesExcludingRequirementAttribute)
        {
            if (attribute is TiHasTags attrAsHasTags)
            {
                var attributeTags = attrAsHasTags.Tags;

                tagsArray.AddRange(attributeTags
                      .Select(s => s.Name)
                      .Except(tagsArray)
                      .ToList());
            }
        }

        return tagsArray;
    }

    public static List<string> GetTiRequirementName(Type typeOfTestCase)
    {
        var attributesTiRequirement = typeOfTestCase
         .GetCustomAttributes(true)
         .Where(a => a is TiRequirement)
         .ToList();

        List<string> tiRequirementString = new();

        foreach (Attribute tiReq in attributesTiRequirement)
        {
            tiRequirementString.Add(((TiRequirement)tiReq).Name);
        }

        return tiRequirementString;
    }
}