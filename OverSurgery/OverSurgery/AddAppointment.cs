﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OverSurgery
{
    class AddAppointment
    {
        #region PROPERTIES
        // Declares an instance of the AddAppointment class.
        private static AddAppointment _instance;

        // List containing all times appointments can be held.
        private List<string> allTimeList = new List<string>();

        // List containing all times that have bookings.
        private List<string> appointmentTimeList = new List<string>();
        
        // List containing all times that are fully booked.
        private List<string> bookedTimeList = new List<string>();

        // List containing all times that can be booked.
        private List<string> possibleTimeList = new List<string>();
        #endregion

        #region METHODS
        /// <summary>
        /// Creates an instance of the AddAppointment class.
        /// </summary>
        /// <returns>Instance of the AddAppointment class.</returns>
        public static AddAppointment GetAddAppointmentInstance()
        {
            if (_instance == null)
                _instance = new AddAppointment();

            return _instance;
        }

        /// <summary>
        /// Finds all appointment records for a particular date.
        /// </summary>
        /// <param name="chosenDate">The chosen date.</param>
        /// <returns></returns>
        public List<string> FindChosenDate(string chosenDate)
        {
            // Gets data set of all distinct appointments for a chosen date.
            DataSet dsAppointment = DatabaseConnection.getDatabaseConnectionInstance().getDataSet(Constants.SelectAllDates(chosenDate));

            // Calls GetBookedTimeList method.
            GetBookedTimesList(dsAppointment, chosenDate);

            return possibleTimeList;
        }

        /// <summary>
        /// Gets a list of all times where all appointments for that time are booked.
        /// </summary>
        /// <param name="dsAppointment"></param>
        /// <returns></returns>
        public void GetBookedTimesList(DataSet dsAppointment, string chosenDate)
        {
            // Checks to see if there are no bookings for the given date.
            if(dsAppointment.Tables[0].Rows.Count > 0)
            {
                GetAppointmentTimeList(dsAppointment, chosenDate);

                AddToBookedTimeList(dsAppointment, chosenDate);
            }
            else
            {
                possibleTimeList = allTimeList;
            }       
        }

        /// <summary>
        /// Creates a list of all times that have appointments booked.
        /// </summary>
        /// <param name="dsAppointment"></param>
        /// <param name="chosenDate"></param>
        public void GetAppointmentTimeList(DataSet dsAppointment, string chosenDate)
        {
            // Loops through all the data set rows.
            for (int i = 0; i < dsAppointment.Tables[0].Rows.Count; i++)
            {
                // Adds each distinct time to the list.
                appointmentTimeList.Add(dsAppointment.Tables[0].Rows[i]["Time"].ToString());

                Console.WriteLine(dsAppointment.Tables[0].Rows[i]["Time"].ToString());
            }

            Console.WriteLine("appointmnet list length" + appointmentTimeList.Count.ToString());
        }

        /// <summary>
        /// Compares the amount of staff with appointments for a particular time and the amount of overall staff
        /// to determine whether there are appointments available at that particular time.
        /// </summary>
        /// <param name="dsAppointment"></param>
        /// <param name="chosenDate"></param>
        public void AddToBookedTimeList(DataSet dsAppointment, string chosenDate)
        {
            // Loops through all the times in the list.
            for (int j = 0; j < appointmentTimeList.Count; j++)
            {
                // Stores the current value in the list.
                string listValue = appointmentTimeList[j].ToString();

                // Gets all the appointments at a given time.
                DataSet dsTimesCount = DatabaseConnection.getDatabaseConnectionInstance().getDataSet(Constants.GetAppointmentsAtGivenTime(chosenDate, listValue));

                // Gets all the staff members.
                DataSet dsStaffCount = DatabaseConnection.getDatabaseConnectionInstance().getDataSet(Constants.GetStaffMembers());

                Console.WriteLine(dsTimesCount.Tables[0].Rows.Count);
                Console.WriteLine(dsStaffCount.Tables[0].Rows.Count);

                // If the number of appointments ata given time is equal to the number of staff
                // available for appointments, the time is fully booked and added to the list.
                if(dsTimesCount.Tables[0].Rows.Count >= dsStaffCount.Tables[0].Rows.Count)
                {
                    bookedTimeList.Add(appointmentTimeList[j]);
                }
            }

            CompareTimeList();
        }

        /// <summary>
        // Creates a list with only times that are not fully booked.
        /// </summary>
        public void CompareTimeList()
        {
            // Creates a list with all the times except ones which are fully booked.
            possibleTimeList = allTimeList.Except(bookedTimeList).ToList();
            PrintTotals();         
        }

        /// <summary>
        /// Creates a list containg all possible appointment times.
        /// </summary>
        public void GetAllTimesList()
        {
            // Sets the hour to 7.
            int hour = 7;

            // Iterates through each time 8-5, adding times at 10 min intervals.
            for (int i = 0; i <= 8; i++)
            {
                // Increments the hour.
                hour++;

                // Converts hour to string.
                string stringHour = hour.ToString();

                // Sets the minute to 0.
                int minute = 0;

                // Iterates through each 10 min slot in each hour,
                // adding it to list.
                for (int j = 0; j <= 5; j++)
                {
                    // Converts minute to string.
                    string stringMinute = minute.ToString();

                    // If the minute = 0, adds an extra 0 so two decimal places.
                    if (stringMinute == "0")
                    {
                        stringMinute = stringMinute + "0";
                    }

                    // Concats string to show hour and minute in correct  format and
                    // adds it to list.
                    allTimeList.Add(stringHour + ":" + stringMinute);

                    // Increments the minute by 10.
                    minute = minute + 10;
                }
            }
        }

        public void PrintTotals()
        {
            Console.WriteLine("ALL TIMES LIST");

            foreach(string s in allTimeList)
            {
                
                Console.WriteLine(s);
            }

            Console.WriteLine("BOOKED TIMES LIST");

            foreach(string s in bookedTimeList)
            {
                
                Console.WriteLine(s);
            }

            Console.WriteLine("POSSIBLE TIMES LIST");

            foreach(string s in possibleTimeList)
            {
                Console.WriteLine(s);
            }
        }

        #endregion

        #region CONSTRUCTORS
        public AddAppointment()
        {
            // Gets a list containing all possible appointment times.
            GetAllTimesList();
        }
        #endregion
    }
}
