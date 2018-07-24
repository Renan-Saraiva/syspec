declare @o varchar(200); set @o = 'SysPec_p_AddPasto';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddPasto(
	@xml xml,
	@pasto int output
) with encryption as
begin

	begin try

		insert into Pastos
		(
			Nome,
			QtdAnimaisSuporte,
			Fazenda,
			Anotacoes,
			Habilitado
		)
		select
			x.n.value('Nome[1]', 'varchar(100)'),
			x.n.value('QtdAnimaisSuporte[1]', 'int'),
			x.n.value('Fazenda[1]', 'int'),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			x.n.value('Habilitado[1]', 'bit')
		from
			@xml.nodes('/*[1]') x(n);

		set @pasto = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
