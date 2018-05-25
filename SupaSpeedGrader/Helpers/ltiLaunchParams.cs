using System;
using System.Collections.Specialized;

namespace SupaSpeedGrader.Helpers
{
	/// <summary>
	/// 2017 - LGE
	/// A class to parse parameters received during an LTI launch request from Canvas
	/// </summary>
	public class ltiLaunchParams
	{
		public string context_id = string.Empty;
		public string context_label = string.Empty;
		public string context_title = string.Empty;
		public string custom_canvas_api_domain = string.Empty;
		public string custom_canvas_course_id = string.Empty;
		public string custom_canvas_enrollment_state = string.Empty;
		public string custom_canvas_user_id = string.Empty;
		public string custom_canvas_user_login_id = string.Empty;
		public string ext_roles = string.Empty;
		public string launch_presentation_document_target = string.Empty;
		public string launch_presentation_height = string.Empty;
		public string launch_presentation_locale = string.Empty;
		public string launch_presentation_return_url = string.Empty;
		public string launch_presentation_width = string.Empty;
		public string lis_course_offering_sourcedid = string.Empty;
		public string lis_person_contact_email_primary = string.Empty;
		public string lis_person_name_family = string.Empty;
		public string lis_person_name_full = string.Empty;
		public string lis_person_name_given = string.Empty;
		public string lis_outcome_service_url = string.Empty;
		public string lis_result_sourcedid = string.Empty;
		public string lti_message_type = string.Empty;
		public string lti_version = string.Empty;
		public string oauth_callback = string.Empty;
		public string oauth_consumer_key = string.Empty;
		public string oauth_nonce = string.Empty;
		public string oauth_signature = string.Empty;
		public string oauth_signature_method = string.Empty;
		public string oauth_timestamp = string.Empty;
		public string oauth_version = string.Empty;
		public string resource_link_id = string.Empty;
		public string resource_link_title = string.Empty;
		public string roles = string.Empty;
		public string tool_consumer_info_product_family_code = string.Empty;
		public string tool_consumer_info_version = string.Empty;
		public string tool_consumer_instance_contact_email = string.Empty;
		public string tool_consumer_instance_guid = string.Empty;
		public string tool_consumer_instance_name = string.Empty;
		public string user_id = string.Empty;
		public string user_image = string.Empty;

		//canvas vars
		public string ext_content_return_types = string.Empty;
		public string ext_content_file_extensions = string.Empty;
		public string selection_directive = string.Empty;

		public ltiLaunchParams()
		{
		}

		public ltiLaunchParams(NameValueCollection formParams)
		{
			parseFormData(formParams);
		}

		public void parseFormData(NameValueCollection formParams)
		{
			this.context_id = (formParams["context_id"] != null) ? formParams["context_id"] : string.Empty;
			this.context_label = (formParams["context_label"] != null) ? formParams["context_label"] : string.Empty;
			this.context_title = (formParams["context_title"] != null) ? formParams["context_title"] : string.Empty;
			this.custom_canvas_api_domain = (formParams["custom_canvas_api_domain"] != null) ? formParams["custom_canvas_api_domain"] : string.Empty;
			this.custom_canvas_course_id = (formParams["custom_canvas_course_id"] != null) ? formParams["custom_canvas_course_id"] : string.Empty;
			this.custom_canvas_enrollment_state = (formParams["custom_canvas_enrollment_state"] != null) ? formParams["custom_canvas_enrollment_state"] : string.Empty;
			this.custom_canvas_user_id = (formParams["custom_canvas_user_id"] != null) ? formParams["custom_canvas_user_id"] : string.Empty;
			this.custom_canvas_user_login_id = (formParams["custom_canvas_user_login_id"] != null) ? formParams["custom_canvas_user_login_id"] : string.Empty;
			this.ext_roles = (formParams["ext_roles"] != null) ? formParams["ext_roles"] : string.Empty;
			this.launch_presentation_document_target = (formParams["launch_presentation_document_target"] != null) ? formParams["launch_presentation_document_target"] : string.Empty;
			this.launch_presentation_height = (formParams["launch_presentation_height"] != null) ? formParams["launch_presentation_height"] : string.Empty;
			this.launch_presentation_locale = (formParams["launch_presentation_locale"] != null) ? formParams["launch_presentation_locale"] : string.Empty;
			this.launch_presentation_return_url = (formParams["launch_presentation_return_url"] != null) ? formParams["launch_presentation_return_url"] : string.Empty;
			this.launch_presentation_width = (formParams["launch_presentation_width"] != null) ? formParams["launch_presentation_width"] : string.Empty;
			this.lis_course_offering_sourcedid = (formParams["lis_course_offering_sourcedid"] != null) ? formParams["lis_course_offering_sourcedid"] : string.Empty;
			this.lis_person_contact_email_primary = (formParams["lis_person_contact_email_primary"] != null) ? formParams["lis_person_contact_email_primary"] : string.Empty;
			this.lis_person_name_family = (formParams["lis_person_name_family"] != null) ? formParams["lis_person_name_family"] : string.Empty;
			this.lis_person_name_full = (formParams["lis_person_name_full"] != null) ? formParams["lis_person_name_full"] : string.Empty;
			this.lis_person_name_given = (formParams["lis_person_name_given"] != null) ? formParams["lis_person_name_given"] : string.Empty;
			this.lis_outcome_service_url = (formParams["lis_outcome_service_url"] != null) ? formParams["lis_outcome_service_url"] : string.Empty;
			this.lis_result_sourcedid = (formParams["lis_result_sourcedid"] != null) ? formParams["lis_result_sourcedid"] : string.Empty;

			this.lti_message_type = (formParams["lti_message_type"] != null) ? formParams["lti_message_type"] : string.Empty;
			this.lti_version = (formParams["lti_version"] != null) ? formParams["lti_version"] : string.Empty;

			this.oauth_callback = (formParams["oauth_callback"] != null) ? formParams["oauth_callback"] : string.Empty;
			this.oauth_consumer_key = (formParams["oauth_consumer_key"] != null) ? formParams["oauth_consumer_key"] : string.Empty;
			this.oauth_nonce = (formParams["oauth_nonce"] != null) ? formParams["oauth_nonce"] : string.Empty;
			this.oauth_signature = (formParams["oauth_signature"] != null) ? formParams["oauth_signature"] : string.Empty;
			this.oauth_signature_method = (formParams["oauth_signature_method"] != null) ? formParams["oauth_signature_method"] : string.Empty;
			this.oauth_timestamp = (formParams["oauth_timestamp"] != null) ? formParams["oauth_timestamp"] : string.Empty;
			this.oauth_version = (formParams["oauth_version"] != null) ? formParams["oauth_version"] : string.Empty;
			this.resource_link_id = (formParams["resource_link_id"] != null) ? formParams["resource_link_id"] : string.Empty;
			this.resource_link_title = (formParams["resource_link_title"] != null) ? formParams["resource_link_title"] : string.Empty;
			this.roles = (formParams["roles"] != null) ? formParams["roles"] : string.Empty;
			this.tool_consumer_info_product_family_code = (formParams["tool_consumer_info_product_family_code"] != null) ? formParams["tool_consumer_info_product_family_code"] : string.Empty;
			this.tool_consumer_info_version = (formParams["tool_consumer_info_version"] != null) ? formParams["tool_consumer_info_version"] : string.Empty;
			this.tool_consumer_instance_contact_email = (formParams["tool_consumer_instance_contact_email"] != null) ? formParams["tool_consumer_instance_contact_email"] : string.Empty;
			this.tool_consumer_instance_guid = (formParams["tool_consumer_instance_guid"] != null) ? formParams["tool_consumer_instance_guid"] : string.Empty;
			this.tool_consumer_instance_name = (formParams["tool_consumer_instance_name"] != null) ? formParams["tool_consumer_instance_name"] : string.Empty;
			this.user_id = (formParams["user_id"] != null) ? formParams["user_id"] : string.Empty;
			this.user_image = (formParams["user_image"] != null) ? formParams["user_image"] : string.Empty;
			this.ext_content_return_types = (formParams["ext_content_return_types"] != null) ? formParams["ext_content_return_types"] : string.Empty;
			this.selection_directive = (formParams["selection_directive"] != null) ? formParams["selection_directive"] : string.Empty;
			this.ext_content_file_extensions = (formParams["ext_content_file_extensions"] != null) ? formParams["ext_content_file_extensions"] : string.Empty;
		}
	}
}