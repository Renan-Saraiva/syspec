declare @o varchar(200); set @o = 'SysPec_p_DisableAnimal';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_DisableAnimal(
	@Id int
) with encryption as
begin

	begin try
		
		update SysPec_c_Animais set
			Habilitado = 0
		where
			Id = @Id

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go