# ObservancesBot
Checks the wikipedia article for today's date, and sends today's "Holidays and observances" section to a Discord webhook. Then exits.

Use in conjunction with [docker-cron](https://github.com/cshtdd/docker-cron) for optimal effect.

# Docker deployment
Use dockerfile in ObservancesBot directory. Environment variables:
- WEBHOOK_URL
