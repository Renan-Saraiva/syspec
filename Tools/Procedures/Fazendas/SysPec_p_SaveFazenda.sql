declare @o varchar(200); set @o = 'SysPec_p_SaveFazenda';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_SaveFazenda(
	@xml xml
) with encryption as
begin

	begin try
		
		update Fazendas set
			Nome = x.n.value('Nome[1]', 'varchar(150)')
		from
			@xml.nodes('/*[1]') x(n)
		where
			Id = x.n.value('Id[1]', 'nvarchar(256)');

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go