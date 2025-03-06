﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pinterest.Database.Posts.Contexts;

#nullable disable

namespace Pinterest.Database.Posts.Migrations
{
    [DbContext(typeof(PostsDbContext))]
    partial class PostsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.Comment", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LikesCount")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ParentCommentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("UserUuid")
                        .HasColumnType("uuid");

                    b.HasKey("Uuid");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("Comment", "public");
                });

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.PostInfo", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorUuid")
                        .HasColumnType("uuid");

                    b.Property<bool>("CommentsEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("FileUuid")
                        .HasColumnType("uuid");

                    b.Property<string>("GrantedAccess")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<int>("LikesCount")
                        .HasColumnType("integer");

                    b.Property<Guid?>("PreviewUuid")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("ViewsCount")
                        .HasColumnType("integer");

                    b.HasKey("Uuid");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("PostInfo", "public");
                });

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.TagInfo", b =>
                {
                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Uuid");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("TagInfo", "public");
                });

            modelBuilder.Entity("PostInfoTagInfo", b =>
                {
                    b.Property<Guid>("PostsUuid")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagsUuid")
                        .HasColumnType("uuid");

                    b.HasKey("PostsUuid", "TagsUuid");

                    b.HasIndex("TagsUuid");

                    b.ToTable("PostTagsConnection", "public");
                });

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.Comment", b =>
                {
                    b.HasOne("Pinterest.Domain.Posts.Entities.Comment", "ParentComment")
                        .WithMany("SubComments")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Pinterest.Domain.Posts.Entities.PostInfo", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentComment");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("PostInfoTagInfo", b =>
                {
                    b.HasOne("Pinterest.Domain.Posts.Entities.PostInfo", null)
                        .WithMany()
                        .HasForeignKey("PostsUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pinterest.Domain.Posts.Entities.TagInfo", null)
                        .WithMany()
                        .HasForeignKey("TagsUuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.Comment", b =>
                {
                    b.Navigation("SubComments");
                });

            modelBuilder.Entity("Pinterest.Domain.Posts.Entities.PostInfo", b =>
                {
                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
