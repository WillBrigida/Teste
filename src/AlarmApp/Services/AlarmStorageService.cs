﻿using System;
using AlarmApp.Models;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Realms;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmApp.Services
{
	public class AlarmStorageService : IAlarmStorageService
	{
		IAlarmSetter AlarmSetter { get; } = DependencyService.Get<IAlarmSetter>();

		public Realm Realm { get { return Realm.GetInstance();} }


		public AlarmStorageService()
		{
			
		}

		public Alarm GetAlarm(string id)
		{
			return Realm.Find<Alarm>(id);
		}

		/// <summary>
		/// Gets all alarms
		/// </summary>
		/// <returns>All alarms</returns>
		public List<Alarm> GetAllAlarms()
		{
			return Realm.All<Alarm>().ToList();
		}

		/// <summary>
		/// Gets the alarms for today
		/// </summary>
		/// <returns>Today's alarms</returns>
		public List<Alarm> GetTodaysAlarms()
		{
			var all = Realm.All<Alarm>();
			return all.ToList().Where(x => x.OccursToday == true).ToList();
		}

		/// <summary>
		/// Adds the alarm
		/// </summary>
		/// <param name="alarm">Alarm to add</param>
		public void AddAlarm(Alarm alarm)
		{
			Realm.Write(() =>
			{
				Realm.Add<Alarm>(alarm);
			});
		}

		/// <summary>
		/// Updates the alarm
		/// </summary>
		/// <param name="alarm">Alarm to update</param>
		public void UpdateAlarm(Alarm alarm)
		{
			Realm.Write(() =>
			{
				Realm.Add<Alarm>(alarm, true);
			});
		}

		/// <summary>
		/// Deletes the alarm
		/// </summary>
		/// <param name="alarm">Alarm we want to delete</param>
		public void DeleteAlarm(Alarm alarm)
		{
			AlarmSetter.DeleteAlarm(alarm);
			Realm.Write(() =>
			{
				Realm.Remove(alarm);
			});
		}

		/// <summary>
		/// Checks if the given alarm exists
		/// </summary>
		/// <returns><c>true</c>, if alarm was found, <c>false</c> otherwise</returns>
		/// <param name="alarm">The Alarm we want to know already exists</param>
		public bool DoesAlarmExist(Alarm alarm)
		{
			var containsAlarm = Realm.All<Alarm>().Contains(alarm);
			if (containsAlarm)
				return true;

			return false;
		}

		/// <summary>
		/// Deletes all the alarms
		/// </summary>
		public void DeleteAllAlarms()
		{
			//remove all from android
			AlarmSetter.DeleteAllAlarms(Realm.All<Alarm>().ToList());
			Realm.Write(() =>
			{
				Realm.RemoveAll<Alarm>();
			});
		}

		/// <summary>
		/// Gets the settings
		/// </summary>
		/// <returns>The settings object</returns>
		public Settings GetSettings()
		{
			Settings settings = new Settings();

			var settingsList =Realm.All<Settings>();
			var settingsAreFound = settingsList?.Count() > 0;

			if (settingsAreFound)
				settings = settingsList.ElementAt(0);
			else
				Realm.Write(() => Realm.Add(settings));

			return settings;
		}

		public static void InitSettings()
		{
			var realm = Realms.Realm.GetInstance();
			Settings settings = new Settings();
			var settingsList = realm.All<Settings>();
			var settingsAreFound = settingsList?.Count() > 0;

			if (settingsAreFound)
				settings = settingsList.ElementAt(0);
			else
				realm.Write(() => realm.Add(settings));
		}


		public List<AlarmTone> GetAllTones()
		{
			return GetSettings().AllAlarmTones.ToList();
		}

		public void AddTone(AlarmTone alarmTone)
		{
			Realm.Write(() =>
			{
				GetSettings().AllAlarmTones.Add(alarmTone);
			});
		}

		public void DeleteTone(AlarmTone alarmTone)
		{
			Realm.Write(() =>
			{
				GetSettings().AllAlarmTones.Remove(alarmTone);
			});
		}

		public void SetDefaultTones()
		{
			var defaults = Defaults.Tones;
			var settings = GetSettings();
			
			Realm.Write(() => {
				foreach (AlarmTone tone in defaults)
				{
					settings.AllAlarmTones.Add(tone);
				}
				settings.AlarmTone = Defaults.Tones[1];
			});
		}

		public AlarmTone GetTone(string id)
		{
			return (AlarmTone) Realm.Find(nameof(AlarmTone), id);
		}
	}
}
