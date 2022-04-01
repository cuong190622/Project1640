namespace Project1640.EF
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddValidate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ideas", "Content", c => c.String(nullable: false));
            AlterColumn("dbo.Comments", "Content", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "Content", c => c.String());
            AlterColumn("dbo.Ideas", "Content", c => c.String());
        }
    }
}
