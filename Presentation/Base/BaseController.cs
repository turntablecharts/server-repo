using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Base
{
    public class BaseController : ControllerBase
    {
        public static ChartResponseDto GetChartResponseDto (Chart thisWeek)
        {
            ChartResponseDto response = new ChartResponseDto ();
            List<ChartHighlightsDto> chartHighlights = new List<ChartHighlightsDto> ();

            ChartDto chartDto = new ChartDto
            {
                Id = thisWeek.Id,
                Week = thisWeek.Week,
                DateCreated = thisWeek.DateCreated,
                Category = thisWeek.Category,
                Genre = thisWeek.Genre,
                HeaderVideoUrl = thisWeek.HeaderVideoUrl,
                IsToDelete = thisWeek.IsToDelete,
                ChartItems = new List<ChartItemDto> ()
            };

            List<ChartItemDto> chartItemDtos = new List<ChartItemDto> ();

            int difference = 0;
            int lowestDifference = 0;

            ChartHighlightsDto biggestGain = new ChartHighlightsDto ();
            ChartHighlightsDto lowestGain = new ChartHighlightsDto ();

            foreach (var chartItem in thisWeek.ChartItems)
            {
                string direction = "";
                if (chartItem.LastPosition == 0)
                {
                    direction = "* New";
                }
                if (chartItem.LastPosition == -1)
                {
                    direction = "* Re-Entry";
                }
                if (chartItem.LastPosition - chartItem.Rank > 0)
                {
                    //&#8593;{{ Number(data.lastPosition) - Number(data.rank) }}
                    var chartDiff = chartItem.LastPosition - chartItem.Rank;
                    if (chartDiff > difference)
                    {
                        biggestGain = new ChartHighlightsDto
                        {
                            Id = chartItem.Id,
                            ChartHighlightTitle = ChartHighlightConstant.BIGGEST_GAIN,
                            ChartItemDto = new ChartItemDto
                            {
                                Id = chartItem.Id,
                                Rank = chartItem.Rank,
                                Artiste = chartItem.Artiste,
                                ImageUri = chartItem.ImageUri
                            }
                        };

                        difference = chartDiff;
                    }
                    direction = "&#8593; " + chartDiff;
                }
                if (chartItem.LastPosition - chartItem.Rank < 0)
                {
                    //  &#8595;{{ Number(data.lastPosition) - Number(data.rank) }}
                    var chartDiff = chartItem.LastPosition - chartItem.Rank;
                    if (chartDiff < lowestDifference)
                    {
                        biggestGain = new ChartHighlightsDto
                        {
                            Id = chartItem.Id,
                            ChartHighlightTitle = ChartHighlightConstant.LOWEST_DROP,
                            ChartItemDto = new ChartItemDto
                            {
                                Id = chartItem.Id,
                                Rank = chartItem.Rank,
                                Artiste = chartItem.Artiste,
                                ImageUri = chartItem.ImageUri
                            }
                        };

                        lowestDifference = chartDiff;
                    }
                    direction = "&#8595; " + lowestDifference;
                }

                string Peak = "";
                if (chartItem.HighestPosition != 0 && chartItem.HighestPosition != -1)
                {
                    Peak = chartItem.HighestPosition.ToString ();
                }
                else
                {
                    Peak = "*";
                }

                string lastPosition = "";
                if (chartItem.LastPosition != 0 && chartItem.LastPosition != -1)
                {
                    lastPosition = chartItem.LastPosition.ToString ();
                }
                else
                {
                    lastPosition = "*";
                }

                chartItemDtos.Add (new ChartItemDto
                {
                    Id = chartItem.Id,
                        Rank = chartItem.Rank,
                        Artiste = chartItem.Artiste,
                        ImageUri = chartItem.ImageUri,
                        LastPosition = lastPosition,
                        Peak = Peak,
                        MusicLink = chartItem.MusicLink,
                        Direction = direction
                });
            }

            chartDto.ChartItems = chartItemDtos.OrderBy (m => m.Rank).ToList ();

            var chartItems = thisWeek.ChartItems;
            var debutChartItems = chartItems.Where (m => m.LastPosition == 0).OrderBy (m => m.Rank).ToList ();

            if (debutChartItems.Count > 0)
            {
                int iterationLength = 3;
                if (debutChartItems.Count < iterationLength)
                {
                    iterationLength = debutChartItems.Count;
                }
                for (int i = 0; i < iterationLength; i++)
                {
                    chartHighlights.Add (new ChartHighlightsDto
                    {
                        Id = debutChartItems[i].Id,
                        ChartHighlightTitle = ChartHighlightConstant.DEBUT,
                        ChartItemDto = new ChartItemDto
                        {
                            Id = debutChartItems[i].Id,
                            Rank = debutChartItems[i].Rank,
                            Artiste = debutChartItems[i].Artiste,
                            ImageUri = debutChartItems[i].ImageUri
                        }

                    });
                }
            }

            chartHighlights.Add (biggestGain);
            int numberOfHighlightsNeeded = 6 - chartHighlights.Count;
            
            if(numberOfHighlightsNeeded == 0)
            {
                response.ChartDto = chartDto;
                response.ChartHighlights = chartHighlights;

                return response;
            }
            else
            {
                if(lowestGain != null)
                {
                    chartHighlights.Add(lowestGain);
                    numberOfHighlightsNeeded--;
                }
                if(numberOfHighlightsNeeded != 0)
                {
                    var reEntryItems = chartItems.Where (m => m.LastPosition == -1).OrderBy (m => m.Rank).ToList();
                    if(reEntryItems.Count > 0)
                    {
                        int iterationLength = numberOfHighlightsNeeded;
                        if (reEntryItems.Count < iterationLength)
                        {
                            iterationLength = reEntryItems.Count;
                        }
                        for (int i = 0; i < iterationLength; i++)
                        {
                            chartHighlights.Add (new ChartHighlightsDto
                            {
                                Id = reEntryItems[i].Id,
                                ChartHighlightTitle = ChartHighlightConstant.RE_ENTRY,
                                ChartItemDto = new ChartItemDto
                                {
                                    Id = reEntryItems[i].Id,
                                    Rank = reEntryItems[i].Rank,
                                    Artiste = reEntryItems[i].Artiste,
                                    ImageUri = reEntryItems[i].ImageUri
                                }

                            });

                            numberOfHighlightsNeeded--;
                        }
                    }

                    if(numberOfHighlightsNeeded != 0)
                    {
                        var lastItems = chartItems.OrderBy (m => m.Rank).TakeLast(numberOfHighlightsNeeded).ToList();
                        foreach (var item in lastItems)
                        {
                            chartHighlights.Add (new ChartHighlightsDto
                            {
                                Id =item.Id,
                                ChartHighlightTitle = ChartHighlightConstant.BOTTOM_FEEDERS,
                                ChartItemDto = new ChartItemDto
                                {
                                    Id = item.Id,
                                    Rank = item.Rank,
                                    Artiste = item.Artiste,
                                    ImageUri = item.ImageUri
                                }

                            });
                        }
                    }
                }

                response.ChartDto = chartDto;
                response.ChartHighlights = chartHighlights;


                return response;

            }

            

            

            // return response;
        }
    }
}