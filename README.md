# ObservancesBot
Checks the wikipedia article for today's date, and sends today's "Holidays and observances" section to a Discord webhook. Then exits. It has a few other sources and targets.

# Docker deployment
Use dockerfile in ObservancesBot directory.

## Command
You can use two commands:
- `dotnet /app/ObservancesBot.dll`: to run once and then exit.
- `/usr/local/bin/schedule`: to configure the above command as a cronjob (see below)

## Environment variables
- `SOURCE`: where to get information. wikipedia (default), daysoftheyear.com, or csv
- `TARGET`: where to send information. discord, or csv
- `ENUMERATE_ALL`: set to any value to enumerate all dates, leave unset to only process today's observances.

The csv source stores data to a csv file. This file can then be read by the csv target. This is ideal if you want to customize another source. For that usecase, use `ENUMERATE_ALL`.

In other situations, you probably shouldn't use `ENUMERATE_ALL`.

If target is discord:
- `WEBHOOK_URL`
- `DISCORD_USE_FIELDS`: if true or unspecified, display observances as embed fields, otherwise use a markdown list.

If target is csv:
- `CSV_PATH`

If source is csv:
- `CSV_PATH`
- `SOURCE_NAME`: the name used to attribute the source (if used by the target, currently only discord)
- `SOURCE_URI_FORMAT`: the format string used to generate the source url (if used by the target, currently only discord) with the only parameter being the DateTime for the observances being processed.

See these documents for explanations on format strings in .NET:
- https://learn.microsoft.com/en-us/dotnet/api/system.string.format?view=net-6.0#remarks-top
- https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
- https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings

For wikipedia, the appropriate format string is this: `https://en.wikipedia.org/wiki/{0:MMMM}_{0:dd}`

## Schedule
The docker image includes cron and a bash script that executes the bot program with the environment variables given to the docker container. It also includes a script that will configure that command to be run as a cronjob (see above). This script reads the environment variable `SCHEDULE` and uses its value as the cron schedule. If not specified, it uses `0 6 * * *`.

Docker containers by default use UTC as their timezone, so you should either keep your schedules in UTC (recommended) or mount /etc/localtime (or whatever timezone file you have) to the container (I have not tested this).
