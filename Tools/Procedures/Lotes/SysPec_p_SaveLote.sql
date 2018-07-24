declare @o varchar(200); set @o = 'SysPec_p_SaveLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_SaveLote(
	@xml xml
) with encryption as
begin

	begin try
		
		update Lotes set
			Nome = x.n.value('Nome[1]', 'varchar(150)'),
			Fazenda = x.n.value('Fazenda[1]', 'int'),
			Anotacoes = x.n.value('Anotacoes[1]', 'varchar(max)'),
			Habilitado = x.n.value('Habilitado[1]', 'bit')
		from
			@xml.nodes('/*[1]') x(n)
		where
			Id = x.n.value('Id[1]', 'int');

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
