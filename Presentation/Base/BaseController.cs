using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
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
                IsToDelete = thisWeek.IsDeleted,
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



        public static List<ChartHighlight> DeductChartHighlight(List<Chart> charts, string category)
        {
            List<ChartHighlight> chartHighlights = new List<ChartHighlight>();


            foreach (var chart in charts)
            {
                if( category != ChartCategoryConst.TOP_50)
                {
                    var result = GetChartHighlight(chart, category);
                    chartHighlights.Add(result);
                }
                else 
                {
                    // var result = GetChartHighlight(chart, category);
                    // chartHighlights.Add(result);
                    var debutResult = BiggestDebut(chart);
                    chartHighlights.Add(debutResult);
                }
            }


            return chartHighlights;
        }

        public static ChartHighlight GetChartHighlight(Chart chart, string category)
        {
            int biggestGain = 0;
            Dictionary<string, string> ChartHighlightDict = new Dictionary<string, string>();
            
            ChartHighlightDict.Add(ChartCategoryConst.TOP_50, ChartHighlightConstant.TOP_50_MOVER);
            ChartHighlightDict.Add(ChartCategoryConst.AIRPLAY, ChartHighlightConstant.BIGGEST_RADIO_MOVER);
            ChartHighlightDict.Add(ChartCategoryConst.TV, ChartHighlightConstant.BIGGEST_TV_MOVER);
            ChartHighlightDict.Add(ChartCategoryConst.STREAMING, ChartHighlightConstant.STREAMING_MOVER);
           

            ChartHighlight response = null;
            var chartItems = chart.ChartItems.Where(m => m.LastPosition != 0);
            var chartItems_1 = chartItems.Where(m => m.LastPosition != -1);
            foreach (var chartItem in chartItems_1)
            {
               var gain = chartItem.LastPosition - chartItem.Rank;
                if(gain > biggestGain)
                    {
                        response = new ChartHighlight
                        {
                            Title = chartItem.Title,
                            Artiste = chartItem.Artiste,
                            ImageUri = chartItem.ImageUri,
                            LastPosition = chartItem.LastPosition,
                            HighestPosition = chartItem.HighestPosition,
                            MusicLink = chartItem.MusicLink,
                            DateCreated = chart.DateCreated,
                            ChartHighlightType = ChartHighlightDict[category],
                            Rank = chartItem.Rank
                        };

                        biggestGain = gain;
                    }
            }

            if(response == null)
            {
                response = new ChartHighlight
                {
                            Title = chart.ChartItems.FirstOrDefault().Title,
                            Artiste = chart.ChartItems.FirstOrDefault().Artiste,
                            ImageUri = chart.ChartItems.FirstOrDefault().ImageUri,
                            LastPosition = chart.ChartItems.FirstOrDefault().LastPosition,
                            HighestPosition = chart.ChartItems.FirstOrDefault().HighestPosition,
                            MusicLink = chart.ChartItems.FirstOrDefault().MusicLink,
                            DateCreated = chart.DateCreated,
                            ChartHighlightType = ChartHighlightDict[category],
                            Rank = chart.ChartItems.FirstOrDefault().Rank
                };
            }

            return response;
        }


        public static ChartHighlight BiggestDebut(Chart chart)
        {
          
            ChartHighlight response = null;

            var debutChartItem = chart.ChartItems.Where (m => m.LastPosition == 0).OrderBy (m => m.Rank).FirstOrDefault ();
            response = new ChartHighlight
            {
                Title = debutChartItem.Title,
                Artiste = debutChartItem.Artiste,
                ImageUri = debutChartItem.ImageUri,
                LastPosition = debutChartItem.LastPosition,
                HighestPosition = debutChartItem.HighestPosition,
                MusicLink = debutChartItem.MusicLink,
                DateCreated = chart.DateCreated,
                ChartHighlightType = ChartHighlightConstant.BIGGEST_DEBUT,
                Rank = debutChartItem.Rank
            };

            return response;
        }


        public static List<ChartHighlight> GetTotalHightLights(DateTime chartDate, IGenericRepository<ChartHighlight> _chartHighlightRepo)
        {
            var chartWeek = chartDate.AddDays(7);
            //var highlights = _chartHighlightRepo.GetWithInclude(m => m.DateCreated);

            DayOfWeek currentDay = chartDate.DayOfWeek;  
            int daysTillCurrentDay = currentDay - DayOfWeek.Sunday;  
            DateTime currentWeekStartDate = chartDate.AddDays(-daysTillCurrentDay);

            DateTime[] weekDates = new DateTime[] {currentWeekStartDate.Date, currentWeekStartDate.AddDays(1).Date, currentWeekStartDate.AddDays(2).Date,currentWeekStartDate.AddDays(3).Date,
                currentWeekStartDate.AddDays(4).Date, currentWeekStartDate.AddDays(5).Date, currentWeekStartDate.AddDays(6).Date};

            var highlights = _chartHighlightRepo.GetWithInclude(m => weekDates.Contains(m.DateCreated.Date), "").ToList();

            var result = highlights.GroupBy(m => m.ChartHighlightType)
                .Select(s => s.First())
                .ToList();


            return result;
        }
    }
}