using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceTrackManagement
{
    class Scheduler
    {
        static void Main(string[] args)
        {
            try
            {
                TrackManagement trackManagement = new TrackManagement();
                List<KeyValuePair<string, int>> talksList = trackManagement.ValidateAndBuildTalks(GetDefaultData());
                trackManagement.ScheduleTracks(talksList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred.." + ex.Message);
                Console.Read();
            }
        }
        public static List<string> GetDefaultData()
        {
            /* Assuming that there is no space between number and min word i.e for example 60min or lightning
               Assuming space between talk title and duration(ex 60min or lightning) inorder to get the duration value correctly. 
               Assuming that there should not be any word after duration(ex 60min or lightning)
               Assuming that duration should be more than 0min and less than or eqaul to 240min(240min is the maximum possible per session) */
            // If any of the above rules are not followed validation errors will be raised.
            // If we want we can add or modify the below talks
            List<string> talks = new List<string>();
            talks.Add("Writing Fast Tests Against Enterprise Rails 60min");
            talks.Add("Overdoing it in Python 45min");
            talks.Add("Lua for the Masses 30min");
            talks.Add("Ruby Errors from Mismatched Gem Versions 45min ");
            talks.Add("Common Ruby Errors 45min");
            talks.Add("Rails for Python Developers lightning");
            talks.Add("Communicating Over Distance 60min");
            talks.Add("Accounting-Driven Development 45min");
            talks.Add("Woah 30min");
            talks.Add("Sit Down and Write 30min");
            talks.Add("Pair Programming vs Noise 45min");
            talks.Add("Rails Magic 60min");
            talks.Add("Ruby on Rails: Why We Should Move On 60min");
            talks.Add("Clojure Ate Scala (on my project) 45min");
            talks.Add("Programming in the Boondocks of Seattle 30min");
            talks.Add("Ruby vs. Clojure for Back-End Development 30min");
            talks.Add("Ruby on Rails Legacy App Maintenance 60min");
            talks.Add("A World Without HackerNews 30min");
            talks.Add("User Interface CSS in Rails Apps 30min");
            
            return talks;
        }
    }
}
