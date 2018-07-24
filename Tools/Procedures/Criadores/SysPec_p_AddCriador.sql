declare @o varchar(200); set @o = 'SysPec_p_AddCriador';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddCriador(
	@xml xml,
	@user int output
) with encryption as
begin

	begin try

		insert into Criadores 
		(
			Nome,
			Usuario,
			Telefone
		)
		select
			x.n.value('Nome[1]', 'varchar(150)'),
			x.n.value('Usuario[1]', 'nvarchar(256)'),
			x.n.value('Telefone[1]', 'varchar(15)')
		from
			@xml.nodes('/*[1]') x(n);

		set @user = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
