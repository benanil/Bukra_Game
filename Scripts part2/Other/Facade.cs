
using MiddleGames.Misc;

namespace Bukra
{ 
    static class Facade
    {
        public static WeatherManager weatherManager => WeatherManager.instance;
        public static GameManager gameManager       => GameManager.instance;
        public static GameMenu gameMenu             => GameMenu.instance;
    }
}
