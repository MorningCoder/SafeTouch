<?php
//本页面处理来自手机端的获取验证信息请求
//返回指定接受验证短信的手机号以及验证码
//如果当前数据库中不存在待验证的手机号码则返回的json中result字段为0

//首先打开数据库连接
$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
if(!$link) 
{
  $error_msg = array();
    $error_msg['result'] = '-1';
	  $error_msg['message'] = "连接失败！原因：" . mysql_error();
	    echo json_encode($error_msg);
		}
		//设置编码
		mysql_query("set names utf8",$link);
		//选择数据库
		if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
		{
		  $error_msg = array();
		    $error_msg['result'] = '-1';
			  $error_msg['message'] = "选择数据库失败！原因：" . mysql_error($link);
			    echo json_encode($error_msg);
				}
				//执行读取操作
				$sql = "select username,confirm_num from ConfirmMsg where is_confirmed = 0";

				$arr = array();
				$index = 0;

				if($res = mysql_query($sql,$link))
				{
				    if(mysql_num_rows($res) == 0)
						{
								//如果当前数据库中不存在待验证的手机号码则返回0
								    	$error_msg = array('result' => '0','message' => '结果为空！');
										    	echo json_encode($error_msg);
												    	exit();
															}
																while ($row = mysql_fetch_assoc($res)) 
																	{
																			$i = $index++;
																					$arr[$i]['username'] = $row['username'];
																							$arr[$i]['confirm_num'] = $row['confirm_num'];
																									$arr[$i]['is_confirmed'] = $row['is_confirmed'];
																										}
																										    echo json_encode($arr);
																											} 
																											else
																											{
																												$error_msg = array('result' => '-1','message' => '获取失败：'.mysql_error());
																													echo json_encode($error_msg);
																													}
																													?>
