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
				Helper.Events.Display.RenderedWorld += OnRenderingHud;
			}
            else
            {
				m_Registered = false;
				Helper.Events.Display.RenderedWorld -= OnRenderingHud;
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
						DrawEntity(farmAnimal, farmAnimal.home);
					}
				}
			}

			if (Config.PetToo)
			{
				Pet pet = GetPet(currentLocation);
				if (pet != null && !pet.lastPetDay.Values.Any(x => x == Game1.Date.TotalDays))
				{
					DrawEntity(pet);
				}
			}
		}


		private void DrawEntity(Character animal, Building home = null)
		{
			float factor = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);

			Vector2 offsetForAnimal = GetOffsetForAnimal(home);

			DrawEntity(Config.Bubble, animal, factor, offsetForAnimal);
			DrawEntity(Config.Heart, animal, factor, offsetForAnimal);
		}

		private void DrawEntity(EntitiesConfig config, Character animal, float factor, Vector2 offset)
		{
			offset += animal.position;
			offset += config.Offset;
			offset += new Vector2(animal.Sprite.getWidth() / 2, factor);

			DrawSprite(config, Game1.GlobalToLocal(Game1.uiViewport, offset));
		}

		private void DrawSprite(EntitiesConfig config, Vector2 position)
        {
			Game1.spriteBatch.Draw(
				Game1.mouseCursors,
				position,
				new Rectangle(config.X, config.Y, config.Width, config.Height),
				config.Color,
				config.Rotation,
				config.Origin,
				config.Scale,
				config.SpriteEffects,
				0f);
		}
	}
}
