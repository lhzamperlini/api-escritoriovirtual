namespace EscritorioVirtual.Application.Whiteboards.DTOs;

public record WhiteboardDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? ZoneId,
    string Name,
    string DocumentData,
    DateTime CreatedAt,
    DateTime LastUpdated
);
