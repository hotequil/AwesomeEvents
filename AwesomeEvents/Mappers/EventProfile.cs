using AutoMapper;
using AwesomeEvents.Entities;
using AwesomeEvents.Models;

namespace AwesomeEvents.Mappers;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<Event, EventViewModel>();
        CreateMap<EventSpeaker, EventSpeakerViewModel>();
        CreateMap<EventInputModel, Event>();
        CreateMap<EventSpeakerInputModel, Event>();
    }
}
