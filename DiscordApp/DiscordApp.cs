namespace DiscordApp;

using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using System.ComponentModel;
using CharaTrack;
using DSharpPlus.SlashCommands;
using System.Numerics;
using System.Linq;

class DiscordApp
{

	const string tokenPattern = @"[\w-]{84}";
	const string tokenPatternWithMFA = @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}";
	const string configDirectory = @"./config/";
	private static Dictionary<string, string>? config;
	public static CharaTrackDatabase database = new CharaTrackDatabase();

	static string? getToken()
	{
		Console.WriteLine("Please enter your Discord bot token: ");
		string? token = Console.ReadLine();

		return token;
	}

	static async Task RestartOrHalt()
	{
		Console.WriteLine("Do you want to restart the application? Press 'Y' to restart or any other key to quit.");
		if(Console.ReadKey().Key == ConsoleKey.Y)
		{
			await Main(new string[0]);
		}
		System.Environment.Exit(1);
	}

	static bool TokenIsValid(string token)
	{
		return (Regex.IsMatch(token, tokenPattern) || Regex.IsMatch(token, tokenPatternWithMFA));
	}

	static void buildConfig()
	{
		Console.WriteLine("Configuration file does not exist or contains an invalid token.");
		Dictionary<string, string> tempConfig = new Dictionary<string, string>();
		string? tempToken = getToken();

		//try to get the token as long as the token is null or doesn't match any of the regexes
		while (String.IsNullOrEmpty(tempToken) || !TokenIsValid(tempToken))
		{
			tempToken = getToken();
		}

		tempConfig.Add("token", tempToken);

		Console.WriteLine("Please enter a preferred command prefix (default !): ");
		tempConfig.Add("commandPrefix", Console.ReadLine() ?? "!");		

		//build the config file
		string serializedTempConfig;

		try
		{
			JsonSerializerOptions serializer = new();
			serializer.WriteIndented = true;
			serializedTempConfig = JsonSerializer.Serialize(tempConfig, serializer);

			File.WriteAllText(configDirectory + "config.json", serializedTempConfig);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

	}

	static DiscordClientBuilder loadConfig()
	{
		//reconstruct the config from JSON
		string configToDeserialize = File.ReadAllText(configDirectory + "config.json");
		config = JsonSerializer.Deserialize<Dictionary<string, string>>(configToDeserialize);

		if(config != null)
		{
			if (!TokenIsValid(config["token"]))
			{
				buildConfig();
				return loadConfig();
			}

			return DiscordClientBuilder.CreateDefault(config["token"], DiscordIntents.AllUnprivileged |
				DiscordIntents.MessageContents | DiscordIntents.GuildMembers | DiscordIntents.GuildPresences | 
				SlashCommandProcessor.RequiredIntents | TextCommandProcessor.RequiredIntents);
		}

		//if the config returned null then we have to rebuilt the config!
		buildConfig();
		return loadConfig();
		
	}

	static ulong getGuildID()
	{
		string? guildID = Environment.GetEnvironmentVariable("DEBUG_GUILD_ID");
		if (String.IsNullOrEmpty(guildID))
			return 0;
		return ulong.Parse(guildID);
	}

	static async Task Main(string[] args)
	{
		if (!File.Exists(configDirectory + "config.json"))
			buildConfig();

		DiscordClientBuilder builder = loadConfig();

		if (config != null)
		{
		
			Console.WriteLine("Config done! Continuing...");

			DiscordClient client = builder.Build();

			CommandsExtension commandsExtension = client.UseCommands(new CommandsConfiguration()
			{
				DebugGuildId = getGuildID(),
				RegisterDefaultCommandProcessors = true

			});

			// Add all commands by scanning the current assembly
			commandsExtension.AddCommands(typeof(DiscordApp).Assembly);
			TextCommandProcessor textCommandProcessor = new(new()
			{
				// The default behavior is that the bot reacts to direct mentions
				// and to the "!" prefix.
				// If you want to change it, you first set if the bot should react to mentions
				// and then you can provide as many prefixes as you want.
				PrefixResolver = new DefaultPrefixResolver(true, config["commandPrefix"]).ResolvePrefixAsync
			});

			SlashCommandProcessor slashCommandProcessor = new SlashCommandProcessor();

			await commandsExtension.AddProcessorAsync(textCommandProcessor);
			await commandsExtension.AddProcessorAsync(slashCommandProcessor);


			await client.ConnectAsync();

			await Task.Delay(-1);
		}

	}
}

public class PingCommand
{
	[Command("ping"), Description("Ping the bot!")]
	public static ValueTask ExecuteAsync(CommandContext context) => context.RespondAsync($"Pong! Latency is {context.Client.Ping}ms.");
}


/*public class MakeCharacterProvider : IAutoCompleteProvider
{
	public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutoCompleteContext context)
	{
		List<DiscordAutoCompleteChoice> choices = new List<DiscordAutoCompleteChoice>
			{
				new DiscordAutoCompleteChoice("Create", "Make a new character."),
				new DiscordAutoCompleteChoice("List", "List your characters."),
				new DiscordAutoCompleteChoice("View", "View a character."),
				new DiscordAutoCompleteChoice("Edit", "Edit a character."),
				new DiscordAutoCompleteChoice("Delete", "Delete a character. (How could you? ;_;)")
			};

		return choices;
	}

	public ValueTask<IReadOnlyDictionary<string, object>> AutoCompleteAsync(AutoCompleteContext context)
	{
		throw new NotImplementedException();
	}
}

[Command("character"), Description("Character commands.")]
public class MakeCharacterChoice
{
	[Command("character"), Description("Character commands.")]
	public async Task ExecuteAsync(CommandContext context, 
		[Autocomplete(typeof(MakeCharacterProvider))][Option("Create", "Make a new character,", true)] string name,
		[Autocomplete(typeof(MakeCharacterProvider))][Option("List", "List your characters.", true)],
		[Autocomplete(typeof(MakeCharacterProvider))][Option("View", "View a character.", true)]

		)	
	{

	}
	
}*/




/*public class MakeCharacterProviderChoice : DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers.IChoiceProvider
{
	private static readonly IReadOnlyDictionary<string, object> options = new Dictionary<string, object>
	{
		["Create"] = 1,
		["List"] = 2,
		["View"] = 3,
		["Edit"] = 4,
		["Delete"] = 5
	};

	public ValueTask<IReadOnlyDictionary<string, object>> ProvideAsync(CommandParameter parameter) => ValueTask.FromResult(options);

	public Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
	{
		throw new NotImplementedException();
	}
}

public class MakeCharacterChoiceOld
{
	[Command("characterOld"), Description("Character commands.")]
	public async ValueTask ExecuteAsync(CommandContext context, [SlashChoiceProvider<MakeCharacterProviderChoice>] int choice, string name)
	{
		ulong userID = context.User.Id;
		//Console.WriteLine(choice + " picked by " + userID.ToString());

		switch(choice)
		{
			case 1:
				{
					

					DiscordApp.database.AddCharacter(name, userID.ToString(), 0.ToString());
					foreach(string key in DiscordApp.database.GetCharacters())
					{
						Console.WriteLine(DiscordApp.database.GetCharacterContents(key, "name"));
					}
					
					return;
				}
			default: throw new NotImplementedException();
				
				

		}
	}
}*/




[SlashCommandGroup("character", "Character commands.")]
public class MakeCharacterChoiceContainer : ApplicationCommandModule
{
	[Command("Create_Character"), Description("Make a new character.")]
	public Task CreateCharacter(CommandContext context, string name, string revision) 
	{
		CharaTrack.Character character = new Character(name, context.User.Id.ToString(), revision);

		if(DiscordApp.database.CheckIfCharacterPresent(character.characterID))
		{
			context.RespondAsync($"A character of this revision already exists! Try again!");
			return Task.CompletedTask;
		}
			
		DiscordApp.database.AddCharacter(character);
		context.RespondAsync("Character " + name + "," +
				" revision " + revision + ", successfully created!");

		foreach (string key in DiscordApp.database.GetCharacters())
		{
			Console.WriteLine(DiscordApp.database.GetCharacterContents(key, "name"));
		}

		return Task.CompletedTask;
	}

	[Command("List_Characters"), Description("List all your characters.")]
	public Task ListCharacters(CommandContext context)
	{
		if(DiscordApp.database.getDatabaseSize() == 0)
		{
			context.RespondAsync("No characters here!");
			return Task.CompletedTask;
		}

		string charList = "";		
		foreach (string key in DiscordApp.database.GetCharacters())
			charList = charList + ("Character " + DiscordApp.database.GetCharacterContents(key, "name") + "," +
				" revision " + DiscordApp.database.GetCharacterContents(key, "revision") + Environment.NewLine);

		{
			Console.WriteLine(charList);
			context.RespondAsync(charList);
		}

		return Task.CompletedTask;
	}

	[SlashCommand("View_Character", "View a character in detail.")]
	public async Task ViewCharacter(CommandContext context, string name)
	{

	}

	[SlashCommand("Edit_Characater", "Edit a character.")]
	public async Task EditCharacter(CommandContext context, string name)
	{

	}

	[SlashCommand("Delete_Character", "Delete a character. (How could you? ;_;)")]
	public async Task DeleteCharacter(CommandContext context, string name)
	{

	}
}