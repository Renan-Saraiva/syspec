declare @o varchar(200); set @o = 'SysPec_p_AddVacina';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddVacina(
	@xml xml,
	@vacina int output
) with encryption as
begin

	begin try

		insert into Vacinas
		(
			Nome,
			Anotacoes,
			Criador
		)
		select
			x.n.value('Nome[1]', 'varchar(150)'),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			x.n.value('Criador[1]', 'int')
		from
			@xml.nodes('/*[1]') x(n);

		set @vacina = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
