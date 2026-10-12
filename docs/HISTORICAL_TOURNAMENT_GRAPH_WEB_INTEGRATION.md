# Historical Tournament Progress Graph — Web Integration

This document describes how the web application should integrate with the athlete historical tournament progress graph API.

## Endpoint

```http
GET /EMembership/v1/HistoricalTournament/GetHistoricalResultsGraph
```

Development gateway:

```text
https://buildup-gateway-dev.buildup-egypt.com/EMembership/v1/HistoricalTournament/GetHistoricalResultsGraph
```

The endpoint requires a valid bearer token and uses the tenant from the authenticated user's claims.

```http
Authorization: Bearer <access-token>
Accept: application/json
```

## Query parameters

| Parameter | Type | Required | Description |
|---|---:|:---:|---|
| `membershipId` | integer | Yes | Membership whose progress will be displayed. |
| `sportId` | integer | Yes | Sport used to limit the member's results. |
| `tournamentIds` | integer array | No | Include only selected tournaments. Repeat the parameter for multiple values. |
| `raceIds` | integer array | No | Include only selected races. Repeat the parameter for multiple values. |
| `dateFrom` | ISO date | No | Include races on or after this date. Recommended format: `YYYY-MM-DD`. |
| `dateTo` | ISO date | No | Include races on or before this date. Recommended format: `YYYY-MM-DD`. |

Example:

```http
GET /EMembership/v1/HistoricalTournament/GetHistoricalResultsGraph?membershipId=882&sportId=8&tournamentIds=18&tournamentIds=21&raceIds=5&dateFrom=2026-01-01&dateTo=2026-12-31
```

Do not send empty values such as `raceIds=`. Omit an optional parameter when no filter is selected.

## Successful response

The project wraps the result in its standard response envelope. The graph series are available in `data`.

```json
{
  "statusCode": 200,
  "meta": null,
  "succeeded": true,
  "message": "Success",
  "errors": null,
  "data": [
    {
      "raceId": 5,
      "race": "100m Freestyle",
      "points": [
        {
          "resultId": 522,
          "tournamentId": 21,
          "tournament": "القياسات الدورية",
          "date": "2026-06-30T00:00:00",
          "time": "00:01:01.0010000",
          "timeInSeconds": 61.001,
          "rank": 1,
          "lane": "3"
        }
      ]
    }
  ]
}
```

### Series fields

| Field | Type | Description |
|---|---:|---|
| `raceId` | integer or null | Stable race identifier. Use it as the series/filter key. |
| `race` | string | Race display name. |
| `points` | array | Results for this race, ordered from oldest to newest. |

### Point fields

| Field | Type | Description |
|---|---:|---|
| `resultId` | integer | Tournament-result identifier. |
| `tournamentId` | integer | Tournament identifier. |
| `tournament` | string | Tournament display name. |
| `date` | ISO datetime | Race date. |
| `time` | TimeSpan string | Original formatted race time. |
| `timeInSeconds` | number | Numeric race time for the chart Y-axis. |
| `rank` | integer or null | Athlete's rank in the race. |
| `lane` | string | Lane value, or `N/A` when unavailable. |

Results without a race date or race time are excluded because they cannot be plotted.

## Recommended chart behavior

- Render one line per item in `data` (one line per race).
- Use `point.date` for the X-axis.
- Use `point.timeInSeconds` for the Y-axis.
- Lower time means better performance.
- Do not combine different race IDs into a single line.
- Use `raceId` as the series key and `race` as its label.
- Show tournament, formatted time, rank, lane, and date in the tooltip.
- Highlight the lowest `timeInSeconds` as the personal best.
- If a series contains one point, show the point and a “More results needed to show progress” message.
- If `data` is empty, show an empty state instead of an empty chart.

The API already returns points in chronological order, but the web application may sort them again defensively.

## JavaScript request example

```js
function appendMany(searchParams, name, values) {
  for (const value of values ?? []) {
    searchParams.append(name, String(value));
  }
}

async function getHistoricalResultsGraph({
  membershipId,
  sportId,
  tournamentIds = [],
  raceIds = [],
  dateFrom,
  dateTo,
  accessToken
}) {
  const query = new URLSearchParams({
    membershipId: String(membershipId),
    sportId: String(sportId)
  });

  appendMany(query, "tournamentIds", tournamentIds);
  appendMany(query, "raceIds", raceIds);

  if (dateFrom) query.set("dateFrom", dateFrom);
  if (dateTo) query.set("dateTo", dateTo);

  const response = await fetch(
    `https://buildup-gateway-dev.buildup-egypt.com/EMembership/v1/HistoricalTournament/GetHistoricalResultsGraph?${query}`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`,
        Accept: "application/json"
      }
    }
  );

  const body = await response.json();

  if (!response.ok || !body.succeeded) {
    throw new Error(body.message || "Unable to load tournament progress");
  }

  return body.data;
}
```

## Mapping to a chart library

```js
const datasets = series.map(item => ({
  id: item.raceId,
  label: item.race,
  data: item.points.map(point => ({
    x: point.date,
    y: point.timeInSeconds,
    result: point
  }))
}));
```

Use the numeric `y` value for chart calculations. Use `result.time` only for display in the tooltip.

## Suggested filter workflow

1. The page receives or selects `membershipId` and `sportId`.
2. Load available tournaments and races using the application's existing lookup APIs.
3. Send selected IDs as repeated `tournamentIds` and `raceIds` parameters.
4. Reload the graph when the user applies filters.
5. Debounce automatic filter changes or use an explicit Apply button.
6. Cancel the previous request when filters change quickly.

## Error and loading states

The web application should handle:

- `401`: access token is missing or expired.
- `403`: the authenticated user is not authorized.
- Empty `data`: no matching historical results.
- Network/server errors: retain the selected filters and offer Retry.
- Slow requests: display a chart skeleton or loading indicator.

## Notes

- This graph endpoint is not paginated because a progress chart needs all matching points.
- Filter arrays use repeated query keys, not a comma-separated value.
- `dateFrom` and `dateTo` are inclusive.
- Arabic tournament/race labels should be rendered with the application's normal RTL support.
- The existing `GetHistoricalResultsByMembership` endpoint remains unchanged and should continue to be used for the paginated results table.
