using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AnimalObserver
{
    internal class ModConfig
    {
        public bool PetToo { get; set; } = true;

        public KeySetting Keys { get; set; } = new KeySetting();

        public EntitiesConfig Bubble { get; set; } = new EntitiesConfig()
        {
            X = 141, 
            Y = 465,
            Width = 20,
            Height = 24,
            Scale = 4f,
            Color = Color.White * 0.65f
        };

        public EntitiesConfig Heart { get; set; } = new EntitiesConfig()
        {
            X = 172,
            Y = 514,
            Width = 9,
            Height = 10,
            Offset = new Vector2(17f, 17f),
            Scale = 5f,
        };

        public Offset Offsets { get; set; } = new Offset();        
    }

    internal class Offset
    {
        public Vector2 BarnAnimal { get; set; } = new Vector2(15f, -67f);

        public Vector2 CoopAnimal { get; set; } = new Vector2(-6f, -88f);

        public Vector2 Pet { get; set; } = new Vector2(22f, -120f);
    }


    internal class KeySetting
    {
        public KeybindList Reload { get; set; } = new KeybindList(SButton.F9);

        public KeybindList Toggle { get; set; } = new KeybindList(SButton.F10);
    }
}
