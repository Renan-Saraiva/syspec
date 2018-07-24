declare @o varchar(200); set @o = 'SysPec_p_AddLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddLote(
	@xml xml,
	@lote int output,
	@now datetime output
) with encryption as
begin

	begin try

		set @now = getdate();

		insert into Lotes 
		(
			Nome,
			Fazenda,
			Anotacoes,
			CriadoEm
		)
		select
			x.n.value('Nome[1]', 'varchar(150)'),
			x.n.value('Fazenda[1]', 'int'),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			@now
		from
			@xml.nodes('/*[1]') x(n);

		set @lote = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
