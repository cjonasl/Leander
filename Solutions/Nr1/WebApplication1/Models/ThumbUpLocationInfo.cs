using System;
using System.Collections.Generic;
using System.Collections;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class ThumbUpLocationInfo
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string LocationAlias { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public ThumbUpLocationInfo() { }

        public ThumbUpLocationInfo(int id, string location, string locationAlias, string title, string note)
        {
            this.Id = id;
            this.Location = location;
            this.LocationAlias = locationAlias;
            this.Title = title;
            this.Note = note;
        }
    }

    public static class ThumbUpLocationInfoUtility
    {
        public static List<ThumbUpLocationInfo> GetLocations(bool isDefaultLocation, out string errorMessage)
        {
            List<ThumbUpLocationInfo> list = new List<ThumbUpLocationInfo>();

            errorMessage = null;

            try
            {
                ArrayList locationName, locationNameSortAlias, id, location, title, note;
                string locationAlias;
                int i, index;

                locationName = ResourceUtility.ReturnActualLocations(isDefaultLocation, out errorMessage, out locationNameSortAlias);

                if (!string.IsNullOrEmpty(errorMessage))
                    return null;

                List<Resource> thumbUpLocationResources = ResourceUtility.ReturnListWithResources(ResourcesType.ThumbUpLocation, out errorMessage);

                id = new ArrayList();
                location = new ArrayList();
                title = new ArrayList();
                note = new ArrayList();

                for(i = 0; i < thumbUpLocationResources.Count; i++)
                {
                    id.Add(thumbUpLocationResources[i].Id);
                    location.Add(thumbUpLocationResources[i].ThumbUpLocation);
                    title.Add(thumbUpLocationResources[i].Title);
                    note.Add(thumbUpLocationResources[i].Note);
                }

                Utility.Sort(locationNameSortAlias, locationName);

                for(i = 0; i < locationName.Count; i++)
                {
                    index = location.IndexOf((string)locationName[i]);

                    if (index == -1)
                    {
                        errorMessage = string.Format("ERROR!! There is no ThumbUpLocation resource for location {0}", (string)locationName[i]);
                        return null;
                    }
                    else
                    {
                        locationAlias = ResourceUtility.ReturnLocationAlias((string)locationName[i], out errorMessage);

                        if (!string.IsNullOrEmpty(errorMessage))
                            return null;

                        list.Add(new ThumbUpLocationInfo((int)id[index], (string)locationName[i], locationAlias, (string)title[index], (string)note[index] ?? ""));
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method GetDefaultLocations! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return list;
        }
    }
}