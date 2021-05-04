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
            Scale = 2f,
            Color = Color.White * 0.65f
        };

        public EntitiesConfig Heart { get; set; } = new EntitiesConfig()
        {
            X = 172,
            Y = 514,
            Width = 9,
            Height = 10,
            Offset = new Vector2(7f, 7f),
            Scale = 3f,
        };

        public Offset Offsets { get; set; } = new Offset();        
    }

    internal class Offset
    {
        public Vector2 BarnAnimal { get; set; } = new Vector2(32f, -64f);

        public Vector2 CoopAnimal { get; set; } = new Vector2(0f, -64f);

        public Vector2 Pet { get; set; } = new Vector2(32f, -64f);
    }


    internal class KeySetting
    {
        public KeybindList Reload { get; set; } = new KeybindList(SButton.F9);

        public KeybindList Toggle { get; set; } = new KeybindList(SButton.F10);
    }
}
