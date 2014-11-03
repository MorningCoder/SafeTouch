<?php
//返回md5编码
$content = $_GET['content'];
if(!isset($content))
{
    echo "content not set";
	    exit();
		}
		echo md5($content);
		?>
