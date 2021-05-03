using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalObserver
{
    public class ModEntry : Mod
	{
		private bool m_Registered;

		private ModConfig Config;


		public override void Entry(IModHelper helper)
		{
			LoadConfig();

			helper.Events.Input.ButtonsChanged += OnButtonsChanged;

			RegisterEvent();
		}

        private void LoadConfig()
        {
			Config = Helper.ReadConfig<ModConfig>();
		}

		private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (Config.Keys.Reload.JustPressed())
			{
				LoadConfig();
			}

			if (Config.Keys.Toggle.JustPressed())
            {
				RegisterEvent();
			}
		}

		private List<FarmAnimal> GetAnimals(object currentLocation)
		{
			List<FarmAnimal> list = null;
			if (currentLocation is Farm farm)
			{
				if (farm.animals == null || farm.animals.Count() <= 0)
				{
					return list;
				}
				list = new List<FarmAnimal>();
				using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = farm.animals.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FarmAnimal item = enumerator.Current;
						list.Add(item);
					}
					return list;
				}
			}
			if (currentLocation is AnimalHouse animalHouse)
			{
				if (animalHouse.animals != null && animalHouse.animals.Count() > 0)
				{
					list = new List<FarmAnimal>();
					foreach (FarmAnimal item2 in animalHouse.animals.Values)
					{
						list.Add(item2);
					}
				}
			}
			return list;


		}

		private Pet GetPet(GameLocation currentLocation)
		{
			foreach (NPC npc in currentLocation.characters)
			{
				if (npc is Pet pet)
				{
					return pet;
				}
			}

			return null;
		}


		public Vector2 GetOffsetForAnimal(Building home)
		{
			if (home == null)
				return Config.Offsets.Pet;

			if (home is Coop)
			{
				return Config.Offsets.CoopAnimal;
			}
			else if (home is Barn)
			{
				return Config.Offsets.BarnAnimal;
			}

			return default;
		}

		private void RegisterEvent()
		{
			if (!m_Registered)
			{
				m_Registered = true;
				Helper.Events.Display.RenderingHud += OnRenderingHud;
			}
            else
            {
				m_Registered = false;
				Helper.Events.Display.RenderingHud -= OnRenderingHud;
			}
		}


		private void OnRenderingHud(object sender, EventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			GameLocation currentLocation = Game1.currentLocation;
			List<FarmAnimal> animals = GetAnimals(currentLocation);
			if (animals != null)
			{
				foreach (FarmAnimal farmAnimal in animals)
				{
					if (!farmAnimal.wasPet)
					{
						ShowEntity(farmAnimal, farmAnimal.home);
					}
				}
			}

			if (Config.PetToo)
			{
				Pet pet = GetPet(currentLocation);
				if (pet != null && !pet.lastPetDay.Values.Any(x => x == Game1.Date.TotalDays))
				{
					ShowEntity(pet);
				}
			}
		}
		

		private void ShowEntity(Character animal, Building home = null)
		{
			float factor = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2)) - 32f;
			
			Vector2 offsetForAnimal = GetOffsetForAnimal(home);

			if (animal != null)
			{
				DrawEntity(Config.Bubble, animal, factor, offsetForAnimal);


				float num = 1f / (factor + 36.001f);
				num = num <= 1f ? num : 1f;

				DrawEntity(Config.Heart, animal, factor, offsetForAnimal, num);
			}
		}



		private void DrawEntity(EntitiesConfig config, Character animal, float factor, Vector2 offset, float colorFactor = 1f)
		{
			offset += config.Offset + animal.Position;

			Game1.spriteBatch.Draw(
				Game1.mouseCursors, 
				Game1.GlobalToLocal(Game1.viewport, new Vector2(animal.Sprite.getWidth() / 2 + offset.X, offset.Y + factor)),
				GetRect(config),
				config.Color * colorFactor, 
				config.Rotation, 
				config.Origin, 
				config.Scale,
				config.SpriteEffects,
				0f);
		}

		public static Rectangle? GetRect(EntitiesConfig config) => new Rectangle(config.X, config.Y, config.Width, config.Height);
	}
}
