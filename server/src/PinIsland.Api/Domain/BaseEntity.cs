using System.ComponentModel.DataAnnotations;

namespace PinIsland.Api.Domain;

public abstract class BaseEntity
{
  [Key]
  public Guid Id { get; private set; } = Guid.NewGuid();

  public DateTime CreatedOn { get; private set; }
  public DateTime? LastModifiedOn { get; private set; }

  public bool IsDeleted { get; private set; }

  public void UpdateCreationProperties(DateTime createdOn)
  {
    CreatedOn = createdOn;
  }

  public void UpdateModifiedProperties(DateTime? lastModifiedOn)
  {
    LastModifiedOn = lastModifiedOn;
  }

  public void UpdateIsDeleted(bool isDeleted)
  {
    IsDeleted = isDeleted;
  }
}