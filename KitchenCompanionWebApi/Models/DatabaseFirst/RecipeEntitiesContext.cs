using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class RecipeEntitiesContext : DbContext
{
    public RecipeEntitiesContext()
    {
    }

    public RecipeEntitiesContext(DbContextOptions<RecipeEntitiesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<MealType> MealTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShoppingList> ShoppingLists { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CAMERON\\SQLEXPRESS02;Database=entities;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__6DB38D4E7A5FDD20");

            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.Category1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Category");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__749FA5A7FCB57B1E");

            entity.Property(e => e.FavoriteId).HasColumnName("Favorite_ID");
            entity.Property(e => e.Favorite1)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("Favorite");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__C9029CABD54DE943");

            entity.Property(e => e.IngredientId).HasColumnName("Ingredient_ID");
            entity.Property(e => e.CookTime)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IngredientName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Photo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Preptime)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Serves)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Stars)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.StoreId).HasColumnName("Store_ID");
            entity.Property(e => e.UnitId).HasColumnName("Unit_ID");

            entity.HasOne(d => d.Store).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ingredien__Store__5FB337D6");

            entity.HasOne(d => d.Unit).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ingredien__Unit___60A75C0F");
        });

        modelBuilder.Entity<MealType>(entity =>
        {
            entity.HasKey(e => e.DishId).HasName("PK__MealType__3B04B49F4AD7BCC3");

            entity.ToTable("MealType");

            entity.Property(e => e.DishId).HasColumnName("Dish_ID");
            entity.Property(e => e.MealType1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MealType");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotifId).HasName("PK__Notifica__39C6C7FF87693A14");

            entity.Property(e => e.NotifId).HasColumnName("Notif_ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("User_ID");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__0959CE39AE54C9B7");

            entity.Property(e => e.RecipeId).HasColumnName("Recipe_ID");
            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.ChefId).HasColumnName("Chef_ID");
            entity.Property(e => e.DishId).HasColumnName("Dish_ID");
            entity.Property(e => e.FavoriteId).HasColumnName("Favorite_ID");
            entity.Property(e => e.IsClone).HasColumnName("Is_Clone");
            entity.Property(e => e.IsDeleted).HasColumnName("Is_deleted");
            entity.Property(e => e.IsSetupRecipe).HasColumnName("is_setup_recipe");
            entity.Property(e => e.RecipeDescription)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RecipeName)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recipes__Categor__6477ECF3");

            entity.HasOne(d => d.Chef).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.ChefId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recipes__Chef_ID__656C112C");

            entity.HasOne(d => d.Dish).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recipes__Dish_ID__66603565");

            entity.HasOne(d => d.Favorite).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.FavoriteId)
                .HasConstraintName("FK__Recipes__Favorit__6754599E");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => new { e.RecipeId, e.IngredientId }).HasName("PK__RecipeIn__A5C9E7F3EA5205BD");

            entity.Property(e => e.RecipeId).HasColumnName("Recipe_ID");
            entity.Property(e => e.IngredientId).HasColumnName("Ingredient_ID");
            entity.Property(e => e.UnitId).HasColumnName("Unit_ID");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecipeIng__Ingre__619B8048");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RecipeIng__Recip__628FA481");

            entity.HasOne(d => d.Unit).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__RecipeIng__Unit___6383C8BA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__D80AB49B2B636157");

            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.Class)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(e => e.ShoppingListId).HasName("PK__Shopping__0659AC3A41810E79");

            entity.ToTable("ShoppingList");

            entity.Property(e => e.ShoppingListId).HasColumnName("shopping_list_id");
            entity.Property(e => e.Category)
                .HasColumnType("text")
                .HasColumnName("category");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsDone).HasColumnName("is_done");
            entity.Property(e => e.UserName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__Stores__A0F15B0172B460FE");

            entity.Property(e => e.StoreId).HasColumnName("Store_ID");
            entity.Property(e => e.StoreName)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.StoreUrl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("StoreURL");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__27556B989560EB30");

            entity.Property(e => e.UnitId).HasColumnName("Unit_ID");
            entity.Property(e => e.Unit1)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Unit");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ChefId).HasName("PK__Users__79039B3CD15927D1");

            entity.Property(e => e.ChefId).HasColumnName("Chef_ID");
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.Created).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IsSetup).HasColumnName("is_setup");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.RealName)
                .HasMaxLength(150)
                .HasColumnName("real_name");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.ShortBio).HasMaxLength(500);
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__Role_ID__68487DD7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
