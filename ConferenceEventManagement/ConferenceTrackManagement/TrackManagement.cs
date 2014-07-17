using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceTrackManagement
{
    class TrackManagement
    {        
        ushort morningSessionSpanInMinutes = 180; // Before lunch session contains a maxmimum of 180 minutes
        ushort afternoonSessionSpanInMinutes = 240; // After lunch session contains a maxmimum of 240 minutes without networking event
        ushort maxPossibleMinsPerSession = 0;
        KeyValuePair<string, int>[][] jaggedScheduledTalksArray;
        TimeSpan noonTime = new TimeSpan(12, 0, 0);
        TimeSpan networkingEventStartTime;
        DateTime time = DateTime.Today;
        ushort morningSessionStartTime = 9; // Morning session starts at 9:00 AM
        ushort afternoonSessionStartTime = 13; // Afternoon session starts at 1:00 PM        

        // Schedules talks into tracks
        public void ScheduleTracks(List<KeyValuePair<string, int>> talksList)
        {
            ushort trackCount = 1;
            // If we want to change or add data to default data we can modify the values in GetDefaultData() method            
            if (talksList.Count > 0)
            {
                talksList.Sort((firstPair, nextPair) =>
                {
                    return (firstPair.Value).CompareTo((nextPair.Value));
                }
                );
                try
                {
                    Console.WriteLine("Track Output \n");
                    while (talksList.Count != 0)
                    {
                        jaggedScheduledTalksArray = new KeyValuePair<string, int>[1][];
                        maxPossibleMinsPerSession = GetMaxNumberOfMinsPossiblePerSession(talksList, morningSessionSpanInMinutes);
                        jaggedScheduledTalksArray = GetSchedule(talksList, maxPossibleMinsPerSession);
                        Console.WriteLine("Track " + trackCount.ToString() + "\n");
                        PrintSchedule(jaggedScheduledTalksArray, morningSessionStartTime);
                        Console.Write(time.Add(noonTime).ToString("hh:mm tt") + "   ");
                        Console.WriteLine("Lunch");
                        talksList = RemoveScheduledTalks(talksList, jaggedScheduledTalksArray);
                        if (talksList.Count > 0)
                        {
                            maxPossibleMinsPerSession = GetMaxNumberOfMinsPossiblePerSession(talksList, afternoonSessionSpanInMinutes);
                            jaggedScheduledTalksArray = GetSchedule(talksList, maxPossibleMinsPerSession);
                            PrintSchedule(jaggedScheduledTalksArray, afternoonSessionStartTime);
                        }
                        if ((afternoonSessionSpanInMinutes - maxPossibleMinsPerSession) >= 60)
                        {
                            networkingEventStartTime = new TimeSpan(16, 0, 0);
                        }
                        else
                        {
                            networkingEventStartTime = new TimeSpan(17, 0, 0);
                        }
                        Console.Write(time.Add(networkingEventStartTime).ToString("hh:mm tt") + "   ");
                        Console.WriteLine("Networking Event \n");
                        talksList = RemoveScheduledTalks(talksList, jaggedScheduledTalksArray);
                        trackCount++;
                    }
                    Console.Read();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occurred.." + ex.Message);
                    Console.Read();
                }
            }
            else
            {
                Console.Read();
            }
        }

        // Gets the scheduled talks list for maximum possible minutes per session
        private KeyValuePair<string, int>[][] GetSchedule(List<KeyValuePair<string, int>> talksList, ushort maxPossibleMinsPerSession)
        {
            bool checkTrackScheduled = false;
            int sumOfTalks = 0;
            KeyValuePair<string, int>[][] jaggedScheduledTalksArray = new KeyValuePair<string, int>[1][];
            for (int i = 0; i < talksList.Count; i++)
            {
                sumOfTalks = talksList[i].Value;
                Stack<KeyValuePair<string, int>> scheduleStack = new Stack<KeyValuePair<string, int>>();
                Stack<int> pushedIndexStack = new Stack<int>();
                scheduleStack.Push(talksList[i]);
                int n = 1;
                if (sumOfTalks == maxPossibleMinsPerSession)
                {
                    jaggedScheduledTalksArray[0] = scheduleStack.ToArray();
                    checkTrackScheduled = true;
                    continue;
                }
                if (checkTrackScheduled)
                {
                    break;
                }

                for (int j = i + n; j < talksList.Count; j++)
                {
                    if (sumOfTalks + talksList[j].Value <= maxPossibleMinsPerSession)
                    {
                        scheduleStack.Push(talksList[j]);
                        pushedIndexStack.Push(j);
                        if (sumOfTalks + talksList[j].Value == maxPossibleMinsPerSession)
                        {
                            jaggedScheduledTalksArray[0] = scheduleStack.ToArray();
                            checkTrackScheduled = true;
                            break;
                        }
                        else
                        {
                            sumOfTalks = sumOfTalks + talksList[j].Value;
                        }
                    }
                    else if (scheduleStack.Count > 2)
                    {
                        j = pushedIndexStack.Pop();
                        sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                    }
                    if (j == (talksList.Count) - 1)
                    {

                        if (scheduleStack.Count > 3)
                        {
                            sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                            sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                            j = pushedIndexStack.Pop();
                            j = pushedIndexStack.Pop();
                        }
                        else if (scheduleStack.Count == 1)
                        {
                            break;
                        }
                        else
                        {
                            sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                            n = n + 1;
                            j = i + n - 1;
                        }
                    }
                }
            }
            return jaggedScheduledTalksArray;
        }

        // Gets the maximum possible number of minutes per session for the given talks
        private ushort GetMaxNumberOfMinsPossiblePerSession(List<KeyValuePair<string, int>> talksList, ushort maxPossibleMinsPerSession)
        {
            bool checkTrackScheduled = false;
            int sumOfTalks = 0;
            List<int> sumOfTalkslist = new List<int>();
            for (int i = 0; i < talksList.Count; i++)
            {
                sumOfTalks = talksList[i].Value;
                Stack<KeyValuePair<string, int>> scheduleStack = new Stack<KeyValuePair<string, int>>();
                Stack<int> pushedIndexStack = new Stack<int>();
                scheduleStack.Push(talksList[i]);
                int n = 1;
                if (sumOfTalks == maxPossibleMinsPerSession)
                {
                    checkTrackScheduled = true;
                    sumOfTalkslist.Add(sumOfTalks);
                    break;
                }
                if (checkTrackScheduled)
                {
                    break;
                }
                if (talksList.Count > 1)
                {
                    for (int j = i + n; j < talksList.Count; j++)
                    {
                        if (sumOfTalks + talksList[j].Value <= maxPossibleMinsPerSession)
                        {
                            scheduleStack.Push(talksList[j]);
                            pushedIndexStack.Push(j);
                            if (sumOfTalks + talksList[j].Value == maxPossibleMinsPerSession)
                            {
                                checkTrackScheduled = true;
                                sumOfTalkslist.Add(maxPossibleMinsPerSession);
                                break;
                            }
                            else
                            {
                                sumOfTalks = sumOfTalks + talksList[j].Value;
                            }
                        }
                        else if (scheduleStack.Count > 2)
                        {
                            j = pushedIndexStack.Pop();
                            sumOfTalkslist.Add(sumOfTalks);
                            sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                        }
                        if (j == (talksList.Count) - 1)
                        {

                            if (scheduleStack.Count > 3)
                            {
                                sumOfTalkslist.Add(sumOfTalks);
                                sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                                sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                                j = pushedIndexStack.Pop();
                                j = pushedIndexStack.Pop();
                            }
                            else if (scheduleStack.Count == 1)
                            {
                                break;
                            }
                            else
                            {
                                sumOfTalkslist.Add(sumOfTalks);
                                sumOfTalks = sumOfTalks - (scheduleStack.Pop().Value);
                                n = n + 1;
                                j = i + n - 1;
                            }
                        }
                    }
                }
                else {
                    if(sumOfTalks <= maxPossibleMinsPerSession)
                      sumOfTalkslist.Add(sumOfTalks);
                }
            }
            sumOfTalkslist.Sort();
            ushort maxPossibleSessionValue=0;
            if(sumOfTalkslist.Count>0)
             maxPossibleSessionValue = (ushort)sumOfTalkslist[sumOfTalkslist.Count - 1];
            return maxPossibleSessionValue;
        }

        // Prints the enitre schedule for all tracks in console
        private void PrintSchedule(KeyValuePair<string, int>[][] jaggedScheduledTalksArray, ushort morOrAfternoonSession)
        {
            DateTime time = DateTime.Today;
            for (int i = 0; i < jaggedScheduledTalksArray.Length; i++)
            {
                TimeSpan start_time = new TimeSpan(morOrAfternoonSession, 0, 0);
                TimeSpan end_time;
                if (jaggedScheduledTalksArray[i] != null)
                {
                    for (int j = 0; j < jaggedScheduledTalksArray[i].Length; j++)
                    {
                        TimeSpan interval = new TimeSpan(0, jaggedScheduledTalksArray[i][j].Value, 0);
                        end_time = start_time.Add(interval);
                        Console.Write(time.Add(start_time).ToString("hh:mm tt") + "   ");
                        start_time = end_time;
                        StringBuilder duration=new StringBuilder();
                        if (jaggedScheduledTalksArray[i][j].Value == 5)
                        {
                            duration.Append("lightning");
                        }
                        else {
                            duration.Append(jaggedScheduledTalksArray[i][j].Value + "min");
                        }
                        Console.WriteLine(jaggedScheduledTalksArray[i][j].Key + " " + duration);
                    }
                }
            }
        }

        // Removes talks from the list once they are scheduled.
        private List<KeyValuePair<string, int>> RemoveScheduledTalks(List<KeyValuePair<string, int>> talksList, KeyValuePair<string, int>[][] jaggedScheduledTalksArray)
        {
            if (jaggedScheduledTalksArray[0]!=null)
            {
                for (int i = 0; i < jaggedScheduledTalksArray[0].Length; i++)
                {
                    KeyValuePair<string, int> talk = jaggedScheduledTalksArray[0][i];
                    talksList.Remove(talk);
                }
            }
            return talksList;
        }

        // Validates the given talks and returns the talks list in key value pair
        public List<KeyValuePair<string, int>> ValidateAndBuildTalks(List<string> talks)
        {
            Dictionary<string, int> talksList = new Dictionary<string, int>();            
            string[] splitValue = new string[1] {"min"};
            string[] splitValueWithSpace = new string[1] { " " };
            bool checkError = false;
            foreach (var talk in talks)
            {
                string[] splittedList = talk.Split(splitValueWithSpace, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder title = new StringBuilder();
                int duration = 0;
                for (int i = 0; i < splittedList.Length; i++)
                {
                    if (i == splittedList.Length - 1)
                    {
                        int result;
                        if (splittedList[i].ToLower() == "lightning")
                        {
                            duration = 5;
                        }
                        else
                        {
                            string[] splitDuration = splittedList[i].Split(splitValue, StringSplitOptions.RemoveEmptyEntries);
                            if (splitDuration.Length == 1)
                            {
                                if (!int.TryParse(splitDuration[0], out result))
                                {
                                    Console.WriteLine("Invalid input : Duration should be for example 60min or lightning " + talk);
                                    checkError = true;
                                    break;
                                }
                                else
                                {
                                    if (Convert.ToInt32(splitDuration[0]) > 240 || Convert.ToInt32(splitDuration[0]) <= 0)
                                    {
                                        Console.WriteLine("Invalid input : Duration should be more than 0min and less than or equal to 240min " + talk);
                                        checkError = true;
                                        break;
                                    }
                                    else
                                    {
                                        duration = Convert.ToInt32(splitDuration[0]);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input : Duration should be for example 60min or lightning" + talk);
                                checkError = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(splittedList[i], @"\d"))
                        {
                            Console.WriteLine("Invalid input : Talk title should not contain numbers " + talk);
                            checkError = true;
                            break;
                        }
                        else
                        {
                            title.Append(splittedList[i]+" ");
                        }
                    }
                }
                if (checkError)
                {
                    talksList.Clear();
                    break;
                }
                else
                {
                    if (talksList.ContainsKey(title.ToString()))
                    {
                        Console.WriteLine("Invalid input : Talk title already exists.Please choose a different tiltle " + talk);
                        talksList.Clear();
                        break;
                    }
                    else
                    {
                        talksList.Add(title.ToString(), duration);
                    }
                }
            }                
                return talksList.ToList();
            }                    
    }
}
