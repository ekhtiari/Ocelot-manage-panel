function SaveData(){
    let _data = $("#frmItems").serialize();
    $.ajax({
        url:'/Config/save',
        type:'post',
        data:_data,
        success:function(e){
            if (e.state==="ok"){
                 window.location.href="/Config/Read"    
            }else{
                alert(e.message)
            }
            
        }
    })
    
}