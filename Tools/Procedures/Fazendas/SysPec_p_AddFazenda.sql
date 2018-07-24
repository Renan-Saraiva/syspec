declare @o varchar(200); set @o = 'SysPec_p_AddFazenda';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddFazenda(
	@xml xml,
	@fazenda int output
) with encryption as
begin

	begin try

		insert into Fazendas 
		(
			Nome,
			Criador,
			Abreviatura
		)
		select
			x.n.value('Nome[1]', 'varchar(150)'),
			x.n.value('Criador[1]', 'int'),
			x.n.value('Abreviatura[1]', 'varchar(3)')
		from
			@xml.nodes('/*[1]') x(n);

		set @fazenda = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
