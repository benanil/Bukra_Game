
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
	private const long ClinetId = 805018063887007784;

	public Discord.Discord discord;

	// Use this for initialization
	void Start()
	{
		discord = new Discord.Discord(ClinetId, (ulong)CreateFlags.Default);
		
		var activityManager = discord.GetActivityManager();
		var activity = new Activity
        {
			State = "This game is developing now",
			Name = "Forest Champ",
		};

		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Result.Ok)
			{
				//Debug.LogError("Everything is fine!");
			}
		});
	}

    private void OnApplicationQuit()
    {
        if (discord != null)
        {
			discord.Dispose();
        }
    }

    void Update()
	{
		discord.RunCallbacks();
	}
}